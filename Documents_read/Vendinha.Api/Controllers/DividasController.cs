using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Vendinha.Api.Data;
using Vendinha.Api.DTOs;
using Vendinha.Api.Models;

namespace Vendinha.Api.Controllers;

// Controller para consultar, criar e quitar as dívidas dos clientes.
[ApiController]
[Route("api")]
public class DividasController : ControllerBase
{
    private readonly VendinhaContext banco;

    // O ASP.NET fornece o contexto do banco ao criar o controller.
    public DividasController(VendinhaContext banco)
    {
        this.banco = banco;
    }

    // Lista o histórico de dívidas de um cliente, das mais novas para as antigas.
    [HttpGet("clientes/{clienteId:int}/dividas")]
    public async Task<ActionResult<List<DividaResposta>>> ListarPorCliente(int clienteId)
    {
        if (!await banco.Clientes.AnyAsync(c => c.Id == clienteId))
            return NotFound(new { mensagem = "Cliente não encontrado." });

        var dividas = await banco.Dividas.AsNoTracking()
            .Where(d => d.ClienteId == clienteId)
            .OrderByDescending(d => d.DataCriacao)
            .ToListAsync();

        return Ok(dividas.Select(ParaResposta).ToList());
    }

    // Só permite criar outra dívida se não houver uma em aberto para o cliente.
    [HttpPost("clientes/{clienteId:int}/dividas")]
    public async Task<ActionResult<DividaResposta>> Criar(int clienteId, DividaEntrada entrada)
    {
        if (!await banco.Clientes.AnyAsync(c => c.Id == clienteId))
            return NotFound(new { mensagem = "Cliente não encontrado." });

        if (await banco.Dividas.AnyAsync(d => d.ClienteId == clienteId && !d.Paga))
            return Conflict(new { mensagem = "O cliente já possui uma dívida em aberto." });

        var divida = new Divida
        {
            ClienteId = clienteId,
            Valor = decimal.Round(entrada.Valor, 2),
            Paga = false,
            DataCriacao = DateTime.UtcNow
        };

        banco.Dividas.Add(divida);
        await banco.SaveChangesAsync();

        return Created($"/api/clientes/{clienteId}/dividas", ParaResposta(divida));
    }

    // Marca a dívida como paga e registra a data do pagamento.
    [HttpPatch("dividas/{id:int}/pagar")]
    public async Task<ActionResult<DividaResposta>> MarcarComoPaga(int id)
    {
        var divida = await banco.Dividas.FindAsync(id);
        if (divida is null)
            return NotFound(new { mensagem = "Dívida não encontrada." });

        if (!divida.Paga)
        {
            divida.Paga = true;
            divida.DataPagamento = DateTime.UtcNow;
            await banco.SaveChangesAsync();
        }

        return Ok(ParaResposta(divida));
    }

    // Converte o modelo do banco no formato público da API.
    private static DividaResposta ParaResposta(Divida divida) =>
        new(divida.Id, divida.ClienteId, divida.Valor, divida.Paga,
            divida.DataCriacao, divida.DataPagamento);
}
