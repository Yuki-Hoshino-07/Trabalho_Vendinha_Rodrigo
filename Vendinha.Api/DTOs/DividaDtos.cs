using System.ComponentModel.DataAnnotations;

namespace Vendinha.Api.DTOs;

public class DividaEntrada
{
    [Range(typeof(decimal), "0.01", "99999999.99", ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Valor { get; set; }
}

public record DividaResposta(
    int Id,
    int ClienteId,
    decimal Valor,
    bool Paga,
    DateTime DataCriacao,
    DateTime? DataPagamento);
