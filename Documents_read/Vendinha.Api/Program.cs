// Este arquivo liga os serviços da API e inicia o servidor web.
using Microsoft.EntityFrameworkCore;
using Vendinha.Api.Data;

var builder = WebApplication.CreateBuilder(args);

// Habilita os controllers para que as classes da pasta Controllers recebam as rotas HTTP.
builder.Services.AddControllers();
// Registra o banco de dados SQLite para que os controllers possam usá-lo.
builder.Services.AddDbContext<VendinhaContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Vendinha")));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    // Cria as tabelas na primeira execução. Depois, os dados ficam no arquivo vendinha.db.
    var banco = scope.ServiceProvider.GetRequiredService<VendinhaContext>();
    banco.Database.EnsureCreated();
}

// Publica as rotas definidas nos controllers e começa a atender requisições.
app.MapControllers();
app.Run();
