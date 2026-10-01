# **Cadastro de Produtos**

## Identificação

- **Nome:** Giulia Rocha Barbizan Alves
- **RM:** 558084

## Evidências da entrega

- [Ver os prints da aplicação](docs/prints/README.md#evidências-do-funcionamento)
- [Acessar o vídeo de apresentação](docs/prints/README.md#vídeo-de-apresentação)

Aplicação desktop desenvolvida em C# com WPF para cadastrar e gerenciar produtos. O projeto implementa um CRUD completo com ADO.NET, SQL Server LocalDB, comandos SQL parametrizados, tratamento de exceções, logs em arquivo e testes unitários.

## Funcionalidades

- Inserção de produtos.
- Listagem de todos os produtos cadastrados.
- Busca de produto por ID.
- Atualização dos dados de um produto.
- Exclusão com confirmação do usuário.
- Validação dos campos preenchidos.
- Registro das operações e dos erros em arquivo.
- Proteção contra SQL Injection por parâmetros tipados.

Cada produto possui os campos `Id`, `Nome`, `Preco`, `Estoque` e `Categoria`.

## Tecnologias utilizadas

- C# e .NET 8
- WPF
- ADO.NET
- SQL Server Express LocalDB
- `Microsoft.Data.SqlClient`
- xUnit
- Git

## Pré-requisitos

O projeto deve ser executado no Windows, pois utiliza WPF e SQL Server LocalDB.

Instale ou confirme os seguintes componentes:

- [Git](https://git-scm.com/downloads)
- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- SQL Server Express LocalDB
- `sqlcmd`, incluído nas ferramentas de linha de comando do SQL Server

Verifique a instalação pelo PowerShell:

```powershell
git --version
dotnet --version
SqlLocalDB info
sqlcmd -?
```

## Instalação desde o clone

### 1. Clonar o repositório

```powershell
git clone https://github.com/Giulia-Rocha/checkpoint-5-csharp.git
cd checkpoint-5-csharp
```

### 2. Iniciar o LocalDB

```powershell
SqlLocalDB start MSSQLLocalDB
```

Se a instância ainda não existir, crie-a e depois inicie:

```powershell
SqlLocalDB create MSSQLLocalDB
SqlLocalDB start MSSQLLocalDB
```

### 3. Criar o banco e a tabela

Na raiz do repositório, execute:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -C -i ".\database\create_database.sql"
```

O script cria o banco `CadastroProdutosDb` e a tabela `Produtos`. Ele é idempotente e pode ser executado novamente sem apagar os registros existentes.

### 4. Restaurar as dependências

```powershell
dotnet restore .\CadastroProdutos.sln
```

### 5. Compilar a solução

```powershell
dotnet build .\CadastroProdutos.sln
```

Uma compilação correta termina com `0 Erro(s)`.

### 6. Executar a aplicação

```powershell
dotnet run --project .\src\CadastroProdutos.Wpf\CadastroProdutos.Wpf.csproj
```

Também é possível abrir `CadastroProdutos.sln` no Visual Studio, definir `CadastroProdutos.Wpf` como projeto de inicialização e pressionar `F5`.

## Como utilizar

1. Preencha nome, preço, estoque e categoria.
2. Clique em **Inserir** para cadastrar o produto.
3. Clique em **Atualizar lista** para visualizar os registros.
4. Informe um ID e clique em **Buscar ID** para localizar um produto.
5. Altere os campos carregados e clique em **Atualizar** para salvar as mudanças.
6. Selecione ou informe um produto, clique em **Excluir** e confirme a operação.

Preço e estoque não aceitam valores negativos. Nome e categoria são obrigatórios.

## Executar os testes

Os testes unitários não dependem do LocalDB. Eles usam uma implementação falsa do Repository para verificar validações e fluxos do serviço sem alterar dados reais.

Para executar todos os testes:

```powershell
dotnet test .\CadastroProdutos.sln
```

Para mostrar o nome e o resultado de cada teste:

```powershell
dotnet test .\CadastroProdutos.sln --logger "console;verbosity=detailed"
```

Para executar somente o projeto de testes:

```powershell
dotnet test .\tests\CadastroProdutos.Tests\CadastroProdutos.Tests.csproj --logger "console;verbosity=detailed"
```

Uma execução correta apresenta `Com falha: 0` e todos os testes como aprovados. No Visual Studio, os mesmos resultados podem ser vistos em **Teste → Gerenciador de Testes → Executar Todos**.

## Configuração do banco

A connection string fica em `src/CadastroProdutos.Wpf/appsettings.json`:

```json
{
  "ConnectionStrings": {
    "ProdutosDatabase": "Server=(localdb)\\MSSQLLocalDB;Database=CadastroProdutosDb;Trusted_Connection=True;TrustServerCertificate=True;"
  }
}
```

Caso outra instância do SQL Server seja utilizada, altere apenas o valor de `ProdutosDatabase`.

## Logs

As consultas, alterações e falhas de banco são registradas em `logs/operacoes.log`, ao lado do executável. Durante o desenvolvimento, o caminho normalmente é:

```text
src/CadastroProdutos.Wpf/bin/Debug/net8.0-windows/logs/operacoes.log
```

Cada linha contém data, horário, nível e descrição da operação. A pasta de logs não é versionada pelo Git.

## Estrutura do projeto

```text
checkpoint-5-csharp/
├── database/
│   └── create_database.sql
├── docs/
│   ├── prints/
│   └── roteiro-video.md
├── src/
│   └── CadastroProdutos.Wpf/
│       ├── Data/
│       ├── Models/
│       └── Services/
├── tests/
│   └── CadastroProdutos.Tests/
├── CadastroProdutos.sln
└── README.md
```

- `Data`: interface e implementação do Repository ADO.NET.
- `Models`: classe que representa o produto.
- `Services`: validações, regras da aplicação e gravação dos logs.
- `MainWindow`: interface WPF e interação com o usuário.
- `tests`: testes unitários xUnit.

## Segurança e acesso a dados

- INSERT, UPDATE e DELETE usam `ExecuteNonQuery`.
- SELECT usa `ExecuteReader`.
- O retorno do `SqlDataReader` é mapeado manualmente para `Produto`.
- Todos os valores informados pelo usuário são enviados por parâmetros SQL tipados.
- Exceções do banco são registradas e convertidas em mensagens amigáveis para a interface.

## Solução de problemas

### O LocalDB não inicia

Confira as instâncias disponíveis:

```powershell
SqlLocalDB info
SqlLocalDB info MSSQLLocalDB
```

Depois tente novamente `SqlLocalDB start MSSQLLocalDB`.

### O banco ou a tabela não existe

Execute novamente:

```powershell
sqlcmd -S "(localdb)\MSSQLLocalDB" -E -C -i ".\database\create_database.sql"
```

### Os pacotes não foram encontrados

```powershell
dotnet restore .\CadastroProdutos.sln
```

Confirme também que existe acesso ao `https://api.nuget.org`.

### Os testes não mostram os nomes

Use o logger detalhado:

```powershell
dotnet test .\CadastroProdutos.sln --logger "console;verbosity=detailed"
```

## Evidências e apresentação

- [Capturas das operações](docs/prints/README.md)
