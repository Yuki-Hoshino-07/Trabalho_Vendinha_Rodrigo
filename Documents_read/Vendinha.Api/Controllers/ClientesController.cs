using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vendinha.Api.Data;
using Vendinha.Api.DTOs;
using Vendinha.Api.Models;
using Vendinha.Api.Services;

namespace Vendinha.Api.Controllers;

// Controller que reúne as operações de cadastro e consulta de clientes.
[ApiController]
[Route("api/clientes")]
public class ClientesController : ControllerBase
{
    private readonly VendinhaContext banco;

    // O ASP.NET fornece o contexto do banco ao criar o controller.
    public ClientesController(VendinhaContext banco)
    {
        this.banco = banco;
    }

    // Lista no máximo 10 clientes, filtra pelo nome e ordena por dívida em aberto.
    [HttpGet]
    public async Task<ActionResult<PaginaClientes>> Listar(
        [FromQuery] int pagina = 1,
        [FromQuery] string? nome = null)
    {
        if (pagina < 1)
            return BadRequest(new { mensagem = "A página deve ser maior que zero." });

        var consulta = banco.Clientes.AsNoTracking().AsQueryable();
        if (!string.IsNullOrWhiteSpace(nome))
            consulta = consulta.Where(c => c.NomeCompleto.Contains(nome));

        var total = await consulta.CountAsync();
        var clientes = await consulta
            .Select(c => new
            {
                Cliente = c,
                TotalEmAberto = c.Dividas.Where(d => !d.Paga).Sum(d => (decimal?)d.Valor) ?? 0
            })
            .OrderByDescending(item => item.TotalEmAberto)
            .ThenBy(item => item.Cliente.NomeCompleto)
            .Skip((pagina - 1) * 10)
            .Take(10)
            .ToListAsync();

        var resposta = clientes.Select(item => ParaResposta(item.Cliente, item.TotalEmAberto)).ToList();
        return Ok(new PaginaClientes(pagina, 10, total, resposta));
    }

    // Busca e devolve um cliente específico; 404 significa que o id não existe.
    [HttpGet("{id:int}")]
    public async Task<ActionResult<ClienteResposta>> Buscar(int id)
    {
        var cliente = await banco.Clientes
            .Include(c => c.Dividas)
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id);

        if (cliente is null)
            return NotFound(new { mensagem = "Cliente não encontrado." });

        return Ok(ParaResposta(cliente, cliente.Dividas.Where(d => !d.Paga).Sum(d => d.Valor)));
    }

    // Valida e grava um novo cliente; 409 informa que o CPF já está cadastrado.
    [HttpPost]
    public async Task<ActionResult<ClienteResposta>> Criar(ClienteEntrada entrada)
    {
        if (!ValidadorCpf.EhValido(entrada.Cpf))
            return BadRequest(new { mensagem = "O CPF informado não é válido." });

        var cpf = ValidadorCpf.ApenasNumeros(entrada.Cpf);
        if (await banco.Clientes.AnyAsync(c => c.Cpf == cpf))
            return Conflict(new { mensagem = "Já existe um cliente cadastrado com esse CPF." });

        if (entrada.DataNascimento!.Value.Date > DateTime.Today)
            return BadRequest(new { mensagem = "A data de nascimento não pode estar no futuro." });

        var cliente = new Cliente
        {
            NomeCompleto = entrada.NomeCompleto.Trim(),
            Cpf = cpf,
            DataNascimento = entrada.DataNascimento.Value.Date,
            Email = string.IsNullOrWhiteSpace(entrada.Email) ? null : entrada.Email.Trim()
        };

        banco.Clientes.Add(cliente);
        await banco.SaveChangesAsync();

        return CreatedAtAction(nameof(Buscar), new { id = cliente.Id }, ParaResposta(cliente, 0));
    }

    // Substitui os dados de um cliente existente.
    [HttpPut("{id:int}")]
    public async Task<ActionResult<ClienteResposta>> Atualizar(int id, ClienteEntrada entrada)
    {
        var cliente = await banco.Clientes.Include(c => c.Dividas).FirstOrDefaultAsync(c => c.Id == id);
        if (cliente is null)
            return NotFound(new { mensagem = "Cliente não encontrado." });

        if (!ValidadorCpf.EhValido(entrada.Cpf))
            return BadRequest(new { mensagem = "O CPF informado não é válido." });

        var cpf = ValidadorCpf.ApenasNumeros(entrada.Cpf);
        if (await banco.Clientes.AnyAsync(c => c.Cpf == cpf && c.Id != id))
            return Conflict(new { mensagem = "Já existe outro cliente cadastrado com esse CPF." });

        if (entrada.DataNascimento!.Value.Date > DateTime.Today)
            return BadRequest(new { mensagem = "A data de nascimento não pode estar no futuro." });

        cliente.NomeCompleto = entrada.NomeCompleto.Trim();
        cliente.Cpf = cpf;
        cliente.DataNascimento = entrada.DataNascimento.Value.Date;
        cliente.Email = string.IsNullOrWhiteSpace(entrada.Email) ? null : entrada.Email.Trim();
        await banco.SaveChangesAsync();

        return Ok(ParaResposta(cliente, cliente.Dividas.Where(d => !d.Paga).Sum(d => d.Valor)));
    }

    // Remove o cliente. O relacionamento configurado também remove suas dívidas.
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Excluir(int id)
    {
        var cliente = await banco.Clientes.FindAsync(id);
        if (cliente is null)
            return NotFound(new { mensagem = "Cliente não encontrado." });

        banco.Clientes.Remove(cliente);
        await banco.SaveChangesAsync();
        return NoContent();
    }

    // Calcula a idade considerando se a pessoa já fez aniversário neste ano.
    private static ClienteResposta ParaResposta(Cliente cliente, decimal totalEmAberto)
    {
        var hoje = DateTime.Today;
        var idade = hoje.Year - cliente.DataNascimento.Year;
        if (cliente.DataNascimento.Date > hoje.AddYears(-idade))
            idade--;

        return new ClienteResposta(cliente.Id, cliente.NomeCompleto, cliente.Cpf,
            cliente.DataNascimento, idade, cliente.Email, totalEmAberto);
    }
}
