# Cadastro de Produtos

Aplicação desktop WPF em C# para cadastrar e gerenciar produtos. O projeto usa ADO.NET diretamente, SQL Server LocalDB, comandos SQL parametrizados, mapeamento manual com `SqlDataReader`, logs em arquivo e testes unitários.

## Requisitos

- Windows 10 ou superior
- .NET 8 SDK
- SQL Server Express LocalDB
- `sqlcmd` (opcional, mas recomendado para executar o script pelo terminal)

## Preparar o banco de dados

Na raiz do projeto, execute:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -C -i ".\database\create_database.sql"
```

O script pode ser executado novamente sem apagar os dados existentes. A conexão está em `src/CadastroProdutos.Wpf/appsettings.json`.

## Executar

```powershell
dotnet restore
dotnet build CadastroProdutos.sln
dotnet run --project .\src\CadastroProdutos.Wpf\CadastroProdutos.Wpf.csproj
```

Na janela principal, use o menu ou os botões para inserir, listar, buscar, atualizar e excluir produtos. Para atualizar ou excluir, informe um ID ou selecione uma linha da tabela.

## Executar os testes

```powershell
dotnet test CadastroProdutos.sln
```

Os testes exercitam as validações e os fluxos do serviço com um Repository falso, sem alterar o banco local.

## Logs

As operações e falhas de banco são registradas em `logs/operacoes.log`, ao lado do executável gerado. Em uma execução de desenvolvimento, o caminho normalmente será:

```text
src/CadastroProdutos.Wpf/bin/Debug/net8.0-windows/logs/operacoes.log
```

## Estrutura

- `src/CadastroProdutos.Wpf`: interface WPF, modelo, serviço e Repository ADO.NET.
- `tests/CadastroProdutos.Tests`: testes unitários xUnit.
- `database/create_database.sql`: criação do banco e da tabela `Produtos`.
- `docs/roteiro-video.md`: roteiro sugerido para a apresentação.
- `docs/prints/README.md`: lista dos prints que devem ser capturados manualmente.

## Segurança e tratamento de erros

Todas as entradas são enviadas ao SQL Server por parâmetros tipados. INSERT, UPDATE e DELETE usam `ExecuteNonQuery`; SELECT usa `ExecuteReader`. Exceções de acesso a dados são registradas e convertidas em mensagens amigáveis para a interface.
