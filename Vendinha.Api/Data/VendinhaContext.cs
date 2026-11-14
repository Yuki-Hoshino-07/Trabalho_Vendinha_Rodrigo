using Microsoft.EntityFrameworkCore;
using Vendinha.Api.Models;

namespace Vendinha.Api.Data;

public class VendinhaContext : DbContext
{
    public VendinhaContext(DbContextOptions<VendinhaContext> options) : base(options)
    {
    }

    public DbSet<Cliente> Clientes => Set<Cliente>();
    public DbSet<Divida> Dividas => Set<Divida>();

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
