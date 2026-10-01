using CadastroProdutos.Wpf.Models;
using CadastroProdutos.Wpf.Services;

namespace CadastroProdutos.Tests;

public sealed class ProdutoServiceTests
{
    [Fact]
    public void Inserir_ComProdutoValido_ChamaRepositorio()
    {
        var repository = new FakeProdutoRepository();
        var service = new ProdutoService(repository);

        OperationResult result = service.Inserir(CreateValidProduct());

        Assert.True(result.Success);
        Assert.Equal(1, repository.InsertCalls);
        Assert.Single(service.Listar());
    }

    [Fact]
    public void Inserir_ComDadosInvalidos_NaoChamaRepositorio()
    {
        var repository = new FakeProdutoRepository();
        var service = new ProdutoService(repository);
        var produto = new Produto { Nome = "", Preco = -1, Estoque = -2, Categoria = "" };

        OperationResult result = service.Inserir(produto);

        Assert.False(result.Success);
        Assert.Equal(0, repository.InsertCalls);
        Assert.Contains("nome", result.Message, StringComparison.OrdinalIgnoreCase);
        Assert.Contains("preço", result.Message, StringComparison.OrdinalIgnoreCase);
    }

    [Fact]
    public void BuscarPorId_QuandoExiste_RetornaProduto()
    {
        var repository = new FakeProdutoRepository();
        var service = new ProdutoService(repository);
        service.Inserir(CreateValidProduct());

        Produto? produto = service.BuscarPorId(1);

        Assert.NotNull(produto);
        Assert.Equal("Teclado", produto.Nome);
    }

    [Fact]
    public void BuscarPorId_ComIdInvalido_NaoConsultaRepositorio()
    {
        var service = new ProdutoService(new FakeProdutoRepository());

        Produto? produto = service.BuscarPorId(0);

        Assert.Null(produto);
    }

    [Fact]
    public void Atualizar_ProdutoExistente_AlteraDados()
    {
        var repository = new FakeProdutoRepository();
        var service = new ProdutoService(repository);
        var produto = CreateValidProduct();
        service.Inserir(produto);
        produto.Nome = "Teclado mecânico";

        OperationResult result = service.Atualizar(produto);

        Assert.True(result.Success);
        Assert.Equal(1, repository.UpdateCalls);
        Assert.Equal("Teclado mecânico", service.BuscarPorId(produto.Id)?.Nome);
    }

    [Fact]
    public void Excluir_ProdutoExistente_RemoveRegistro()
    {
        var repository = new FakeProdutoRepository();
        var service = new ProdutoService(repository);
        var produto = CreateValidProduct();
        service.Inserir(produto);

        OperationResult result = service.Excluir(produto.Id);

        Assert.True(result.Success);
        Assert.Equal(1, repository.DeleteCalls);
        Assert.Empty(service.Listar());
    }

    [Theory]
    [InlineData(-1, 0, "Categoria")]
    [InlineData(10, -1, "Categoria")]
    [InlineData(10, 1, "")]
    public void Validar_ComValoresInvalidos_RetornaErros(decimal preco, int estoque, string categoria)
    {
        Produto produto = CreateValidProduct();
        produto.Preco = preco;
        produto.Estoque = estoque;
        produto.Categoria = categoria;

        IReadOnlyList<string> errors = ProdutoValidator.Validate(produto);

        Assert.NotEmpty(errors);
    }

    private static Produto CreateValidProduct() => new()
    {
        Nome = "Teclado",
        Preco = 199.90m,
        Estoque = 12,
        Categoria = "Periféricos"
    };
}
