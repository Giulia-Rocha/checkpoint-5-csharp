using System.Data;
using System.Data.Common;
using CadastroProdutos.Wpf.Models;
using CadastroProdutos.Wpf.Services;
using Microsoft.Data.SqlClient;

namespace CadastroProdutos.Wpf.Data;

public sealed class ProdutoRepository : IProdutoRepository
{
    private readonly string _connectionString;
    private readonly IOperationLogger _logger;

    public ProdutoRepository(string connectionString, IOperationLogger logger)
    {
        _connectionString = connectionString;
        _logger = logger;
    }

    public bool Inserir(Produto produto)
    {
        const string sql = """
            INSERT INTO Produtos (Nome, Preco, Estoque, Categoria)
            VALUES (@Nome, @Preco, @Estoque, @Categoria);
            """;

        return ExecuteWrite("inserir produto", sql, command => AddProductParameters(command, produto));
    }

    public IReadOnlyList<Produto> Listar()
    {
        const string sql = "SELECT Id, Nome, Preco, Estoque, Categoria FROM Produtos ORDER BY Id;";
        var produtos = new List<Produto>();

        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            while (reader.Read())
            {
                produtos.Add(MapProduto(reader));
            }

            _logger.Info($"Produtos listados. Total: {produtos.Count}.");
            return produtos;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw HandleException("listar produtos", exception);
        }
    }

    public Produto? BuscarPorId(int id)
    {
        const string sql = "SELECT Id, Nome, Preco, Estoque, Categoria FROM Produtos WHERE Id = @Id;";

        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            command.Parameters.Add("@Id", SqlDbType.Int).Value = id;
            connection.Open();
            using SqlDataReader reader = command.ExecuteReader();
            Produto? produto = reader.Read() ? MapProduto(reader) : null;
            _logger.Info($"Busca por produto de ID {id}. Encontrado: {produto is not null}.");
            return produto;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw HandleException("buscar produto", exception);
        }
    }

    public bool Atualizar(Produto produto)
    {
        const string sql = """
            UPDATE Produtos
               SET Nome = @Nome, Preco = @Preco, Estoque = @Estoque, Categoria = @Categoria
             WHERE Id = @Id;
            """;

        return ExecuteWrite("atualizar produto", sql, command =>
        {
            AddProductParameters(command, produto);
            command.Parameters.Add("@Id", SqlDbType.Int).Value = produto.Id;
        });
    }

    public bool Excluir(int id)
    {
        const string sql = "DELETE FROM Produtos WHERE Id = @Id;";
        return ExecuteWrite("excluir produto", sql,
            command => command.Parameters.Add("@Id", SqlDbType.Int).Value = id);
    }

    private bool ExecuteWrite(string operation, string sql, Action<SqlCommand> configureCommand)
    {
        try
        {
            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(sql, connection);
            configureCommand(command);
            connection.Open();
            int affectedRows = command.ExecuteNonQuery();
            _logger.Info($"Operação '{operation}' concluída. Registros afetados: {affectedRows}.");
            return affectedRows == 1;
        }
        catch (Exception exception) when (IsDatabaseException(exception))
        {
            throw HandleException(operation, exception);
        }
    }

    private static void AddProductParameters(SqlCommand command, Produto produto)
    {
        command.Parameters.Add("@Nome", SqlDbType.NVarChar, 120).Value = produto.Nome;
        var priceParameter = command.Parameters.Add("@Preco", SqlDbType.Decimal);
        priceParameter.Precision = 18;
        priceParameter.Scale = 2;
        priceParameter.Value = produto.Preco;
        command.Parameters.Add("@Estoque", SqlDbType.Int).Value = produto.Estoque;
        command.Parameters.Add("@Categoria", SqlDbType.NVarChar, 80).Value = produto.Categoria;
    }

    private static Produto MapProduto(SqlDataReader reader) => new()
    {
        Id = reader.GetInt32(reader.GetOrdinal("Id")),
        Nome = reader.GetString(reader.GetOrdinal("Nome")),
        Preco = reader.GetDecimal(reader.GetOrdinal("Preco")),
        Estoque = reader.GetInt32(reader.GetOrdinal("Estoque")),
        Categoria = reader.GetString(reader.GetOrdinal("Categoria"))
    };

    private DatabaseOperationException HandleException(string operation, Exception exception)
    {
        _logger.Error($"Falha ao {operation}.", exception);
        return new DatabaseOperationException(
            $"Não foi possível {operation}. Verifique a conexão com o banco de dados.", exception);
    }

    private static bool IsDatabaseException(Exception exception) =>
        exception is DbException or InvalidOperationException;
}
