namespace Vendinha.Api.Models;

// Representa uma dívida ligada a exatamente um cliente.
public class Divida
{
    // Identificador gerado pelo banco.
    public int Id { get; set; }
    // Chave estrangeira que aponta para o cliente dono da dívida.
    public int ClienteId { get; set; }
    // Propriedade de navegação usada pelo Entity Framework para o relacionamento.
    public Cliente? Cliente { get; set; }
    // Quantia da compra pendurada.
    public decimal Valor { get; set; }
    // False significa em aberto; true significa que foi paga.
    public bool Paga { get; set; }
    // Momento em que a dívida foi cadastrada.
    public DateTime DataCriacao { get; set; }
    // Continua nulo até a dívida ser paga.
    public DateTime? DataPagamento { get; set; }
}
