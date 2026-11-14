# Vendinha - API de clientes e dívidas

Projeto de backend em C# com ASP.NET Core 8 e SQLite. A API guarda clientes e suas dívidas, permite marcar dívidas como pagas e oferece uma listagem paginada dos clientes.

## Requisitos

- .NET 8 SDK (ou mais recente)
- Visual Studio Code e a extensão C# Dev Kit (opcional, mas ajuda a editar)

## Como executar no VS Code

1. Abra a pasta `Vendinha.Api` no VS Code.
2. Abra o terminal integrado em **Terminal > Novo Terminal**.
3. Execute `dotnet restore` para baixar as dependências.
4. Execute `dotnet run`.
5. Copie o endereço local mostrado no terminal, por exemplo `http://localhost:5000`, e use `/api` nas rotas abaixo.

O arquivo `vendinha.db` é criado automaticamente na primeira execução. O script que descreve as tabelas está em `database/schema.sql`.

## Rotas

| Método | Rota | Uso |
|---|---|---|
| GET | `/api/clientes?pagina=1&nome=ana` | Lista clientes, filtra pelo nome e traz 10 por página. Ordena pelo total em aberto (maior primeiro). |
| GET | `/api/clientes/1` | Busca um cliente pelo identificador. |
| POST | `/api/clientes` | Cadastra um cliente. |
| PUT | `/api/clientes/1` | Atualiza os dados de um cliente. |
| DELETE | `/api/clientes/1` | Exclui um cliente e suas dívidas. |
| GET | `/api/clientes/1/dividas` | Lista as dívidas de um cliente. |
| POST | `/api/clientes/1/dividas` | Registra uma dívida em aberto. |
| PATCH | `/api/dividas/1/pagar` | Marca uma dívida como paga. |

Exemplo de corpo para cadastrar um cliente:

```json
{
  "nomeCompleto": "Ana Souza",
  "cpf": "529.982.247-25",
  "dataNascimento": "2000-05-12",
  "email": "ana@example.com"
}
```

Exemplo de corpo para cadastrar uma dívida:

```json
{
  "valor": 45.90
}
```

O e-mail pode ser omitido. O CPF é validado e armazenado somente com números. A idade é calculada na resposta. O total em aberto considera apenas dívidas não pagas.

## Exemplos no terminal

Troque a URL abaixo pela que apareceu ao iniciar a aplicação.

```bash
curl -X POST http://localhost:5000/api/clientes -H "Content-Type: application/json" -d '{"nomeCompleto":"Ana Souza","cpf":"529.982.247-25","dataNascimento":"2000-05-12","email":"ana@example.com"}'
curl "http://localhost:5000/api/clientes?pagina=1&nome=Ana"
curl -X POST http://localhost:5000/api/clientes/1/dividas -H "Content-Type: application/json" -d '{"valor":45.90}'
curl -X PATCH http://localhost:5000/api/dividas/1/pagar
```

## Decisões simples do projeto

- `Cliente` e `Divida` representam as tabelas e o relacionamento um-para-muitos.
- `VendinhaContext` configura o SQLite e as regras de relacionamento.
- Os DTOs definem os dados aceitos e devolvidos pela API.
- A regra de apenas uma dívida em aberto aparece na API e também em um índice único no SQLite.
- O CPF é único no banco, mesmo quando chega formatado com pontos e traço.
