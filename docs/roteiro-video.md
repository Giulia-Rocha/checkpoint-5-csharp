# Roteiro de apresentação e teste — 3 a 5 minutos

## 1. Introdução — 20 segundos

- Apresente o objetivo: aplicação WPF para gerenciar produtos com CRUD completo.
- Informe que o projeto usa C#, ADO.NET e SQL Server LocalDB.

## 2. Estrutura técnica — 40 segundos

- Mostre rapidamente `Produto`, `ProdutoService` e `ProdutoRepository`.
- Destaque a separação entre interface, regras/validações e acesso ao banco.
- Mostre a connection string em `appsettings.json`.
- Mostre um comando SQL parametrizado e o mapeamento manual do `SqlDataReader`.

## 3. Demonstração do CRUD — 2 minutos

1. **Inserir:** cadastre `Teclado Mecânico`, preço `249,90`, estoque `10`, categoria `Periféricos`.
2. **Listar:** atualize a lista e mostre o produto persistido.
3. **Buscar:** informe o ID criado e use “Buscar produto por ID”.
4. **Atualizar:** altere o estoque para `8` e confirme a atualização.
5. **Excluir:** selecione o produto, exclua, confirme a caixa de diálogo e atualize a lista.

## 4. Segurança e erros — 40 segundos

- Tente inserir preço negativo ou campo obrigatório vazio e mostre a validação.
- Explique que uma entrada como `' OR 1=1 --` é tratada como texto pelo parâmetro SQL.
- Abra `logs/operacoes.log` e mostre as operações registradas com data e hora.

## 5. Testes e encerramento — 30 segundos

- Execute `dotnet test CadastroProdutos.sln` e mostre os testes aprovados.
- Recapitule: CRUD, SQL parametrizado, tratamento de exceções, separação em camadas e logs.

## Checklist antes de gravar

- Banco criado pelo script e aplicação iniciando sem erros.
- Lista vazia ou preparada para a demonstração.
- Terminal aberto na raiz do projeto.
- Arquivo de log acessível.
- Resolução da tela suficiente para mostrar formulário e tabela.
