namespace Vendinha.Api.Models;

public class Cliente
{
    public int Id { get; set; }
    public string NomeCompleto { get; set; } = string.Empty;
    public string Cpf { get; set; } = string.Empty;
    public DateTime DataNascimento { get; set; }
    public string? Email { get; set; }
    public List<Divida> Dividas { get; set; } = new List<Divida>();
}
