# Guia de estudo e apresentação

## O que o projeto faz

É um backend para uma vendinha. O dono pode cadastrar clientes, registrar uma dívida em aberto, consultar o histórico e marcar uma dívida como paga. Os dados ficam no SQLite, em `vendinha.db`.

## Como explicar o caminho de uma requisição

1. O cliente HTTP envia uma requisição para uma rota, por exemplo `POST /api/clientes`.
2. O controller recebe os dados e faz as validações de negócio.
3. O `VendinhaContext` usa o Entity Framework Core para consultar ou alterar o SQLite.
4. O controller devolve um código HTTP e, quando necessário, um JSON de resposta.

## Arquivos importantes

- `Program.cs`: configura a API, a conexão SQLite e inicia o servidor.
- `Models/Cliente.cs` e `Models/Divida.cs`: representam os dados persistidos.
- `Data/VendinhaContext.cs`: configura tabelas, índices e relacionamento entre cliente e dívidas.
- `DTOs`: definem o formato de entrada e resposta da API. A idade não é gravada; é calculada ao responder.
- `Controllers/ClientesController.cs`: cadastro, edição, exclusão, consulta e listagem paginada de clientes.
- `Controllers/DividasController.cs`: criação, consulta e pagamento de dívidas.
- `Services/ValidadorCpf.cs`: verifica o CPF pelos dois dígitos verificadores.
- `database/schema.sql`: script que cria manualmente as tabelas e as restrições do banco.

## Rotas para demonstrar

| Operação | Método e rota |
|---|---|
| Criar cliente | `POST /api/clientes` |
| Listar clientes | `GET /api/clientes?pagina=1&nome=Ana` |
| Consultar um cliente | `GET /api/clientes/1` |
| Atualizar cliente | `PUT /api/clientes/1` |
| Excluir cliente | `DELETE /api/clientes/1` |
| Ver dívidas do cliente | `GET /api/clientes/1/dividas` |
| Criar dívida | `POST /api/clientes/1/dividas` |
| Marcar dívida paga | `PATCH /api/dividas/1/pagar` |

Exemplo de cliente:

```json
{
  "nomeCompleto": "Ana Souza",
  "cpf": "529.982.247-25",
  "dataNascimento": "2000-05-12",
  "email": "ana@example.com"
}
```

Exemplo de dívida:

```json
{
  "valor": 45.90
}
```

## Perguntas que podem aparecer

**Por que usar DTOs?**  
Para separar o formato que a API recebe/devolve das classes usadas para salvar no banco. Assim, por exemplo, a idade calculada não precisa virar uma coluna.

**O que é CRUD?**  
As quatro operações básicas: criar (Create), consultar (Read), atualizar (Update) e excluir (Delete). Neste projeto elas aparecem nas rotas de clientes.

**O que o Entity Framework Core faz?**  
É um ORM: permite consultar e salvar objetos C# no banco sem escrever SQL para cada operação. O `VendinhaContext` configura como as classes correspondem às tabelas.

**Por que SQLite?**  
É um banco simples que guarda os dados em um arquivo local, então o projeto não exige instalar ou configurar um servidor de banco separado.

**Por que CPF é salvo sem pontuação?**  
Para que `52998224725` e `529.982.247-25` sejam tratados como o mesmo CPF e não passem como cadastros diferentes.

**Como é calculada a idade?**  
Subtrai o ano de nascimento do ano atual e desconta um ano se o aniversário ainda não aconteceu neste ano.

**Como a regra de uma dívida aberta é garantida?**  
O controller verifica antes de inserir, e o SQLite também tem um índice único parcial para impedir duas dívidas não pagas para o mesmo cliente.

**O que significam os códigos HTTP mais usados aqui?**  
`200` indica sucesso; `201`, criação; `204`, exclusão sem corpo de resposta; `400`, dados inválidos; `404`, registro não encontrado; e `409`, conflito como CPF duplicado ou dívida já aberta.

**Por que a listagem usa páginas de 10?**  
Para não devolver todos os clientes de uma vez. `pagina=1` retorna os dez primeiros; `pagina=2` os próximos dez.

**O que significa `async` e `await`?**  
São usados nas operações de banco para que a aplicação não fique bloqueada esperando cada consulta terminar. `await` espera a operação assíncrona e continua quando o resultado chega.

**O que acontece ao excluir um cliente?**  
O relacionamento está configurado com exclusão em cascata, então as dívidas ligadas a esse cliente também são removidas.

## Execução rápida

Abra `Vendinha.Api` no VS Code, abra o terminal e rode:

```bash
dotnet restore
dotnet run
```

Use o endereço `http://localhost:...` que aparecer no terminal, junto com as rotas acima. Para testar POST e PATCH, use uma ferramenta HTTP como Thunder Client no VS Code ou `curl`.
