using System.ComponentModel.DataAnnotations;

namespace Vendinha.Api.DTOs;

// Corpo simples recebido ao cadastrar uma dívida.
public class DividaEntrada
{
    [Range(typeof(decimal), "0.01", "99999999.99", ErrorMessage = "O valor deve ser maior que zero.")]
    public decimal Valor { get; set; }
}

// Formato usado para devolver os dados da dívida pela API.
public record DividaResposta(
    int Id,
    int ClienteId,
    decimal Valor,
    bool Paga,
    DateTime DataCriacao,
    DateTime? DataPagamento);
