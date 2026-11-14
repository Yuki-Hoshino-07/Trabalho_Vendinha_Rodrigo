using System.ComponentModel.DataAnnotations;

namespace Vendinha.Api.DTOs;

// DTO significa objeto de transferência: descreve os campos que a API recebe.
public class ClienteEntrada
{
    [Required(ErrorMessage = "O nome completo é obrigatório.")]
    [StringLength(150)]
    public string NomeCompleto { get; set; } = string.Empty;

    [Required(ErrorMessage = "O CPF é obrigatório.")]
    public string Cpf { get; set; } = string.Empty;

    [Required(ErrorMessage = "A data de nascimento é obrigatória.")]
    public DateTime? DataNascimento { get; set; }

    [EmailAddress(ErrorMessage = "Informe um e-mail válido.")]
    [StringLength(200)]
    public string? Email { get; set; }
}

// Este DTO controla exatamente os dados enviados ao usuário.
// Idade e total em aberto são calculados pela API, não recebidos do cliente.
public record ClienteResposta(
    int Id,
    string NomeCompleto,
    string Cpf,
    DateTime DataNascimento,
    int Idade,
    string? Email,
    decimal TotalEmAberto);

// Resumo da paginação: número da página, tamanho, total encontrado e resultados.
public record PaginaClientes(int Pagina, int TamanhoPagina, int Total, List<ClienteResposta> Dados);
