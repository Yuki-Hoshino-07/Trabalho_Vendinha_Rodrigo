using Microsoft.EntityFrameworkCore;
using Vendinha.Api.Models;

namespace Vendinha.Api.Data;

// O DbContext é a ponte entre as classes C# e as tabelas SQLite.
public class VendinhaContext : DbContext
{
    public VendinhaContext(DbContextOptions<VendinhaContext> options) : base(options)
    {
    }

    // Coleções que o Entity Framework transforma em tabelas.
    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Divida> Dividas => Set<Divida>();

    // Configura tamanho dos campos, CPF único, relação e regra das dívidas.
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Cliente>(cliente =>
        {
            cliente.Property(c => c.NomeCompleto).HasMaxLength(150).IsRequired();
            cliente.Property(c => c.Cpf).HasMaxLength(11).IsRequired();
            cliente.HasIndex(c => c.Cpf).IsUnique();
            cliente.Property(c => c.Email).HasMaxLength(200);
            cliente.HasMany(c => c.Dividas)
                .WithOne(d => d.Cliente)
                .HasForeignKey(d => d.ClienteId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        modelBuilder.Entity<Divida>(divida =>
        {
            divida.Property(d => d.Valor).HasColumnType("decimal(10,2)");
            // O índice parcial impede duas dívidas não pagas para o mesmo cliente.
            divida.HasIndex(d => d.ClienteId)
                .IsUnique()
                .HasFilter("Paga = 0");
        });
    }
}
