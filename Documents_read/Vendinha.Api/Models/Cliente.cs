namespace Vendinha.Api.Models;

// Representa um cliente salvo no banco.
public class Cliente
{
    // Identificador gerado automaticamente pelo banco.
    public int Id { get; set; }
    // Nome obrigatório informado no cadastro.
    public string NomeCompleto { get; set; } = string.Empty;
    // CPF normalizado, guardado sem pontuação para evitar duplicidade de formatos.
    public string Cpf { get; set; } = string.Empty;
    // Data usada para calcular a idade no momento da resposta.
    public DateTime DataNascimento { get; set; }
    // Campo opcional para contato.
    public string? Email { get; set; }
    // Um cliente pode ter várias dívidas ao longo do tempo.
    public List<Divida> Dividas { get; set; } = new List<Divida>();
}
