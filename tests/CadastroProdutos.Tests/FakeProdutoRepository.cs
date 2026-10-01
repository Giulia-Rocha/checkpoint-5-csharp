using CadastroProdutos.Wpf.Data;
using CadastroProdutos.Wpf.Models;

namespace CadastroProdutos.Tests;

internal sealed class FakeProdutoRepository : IProdutoRepository
{
    private readonly List<Produto> _produtos = [];

    public int InsertCalls { get; private set; }
    public int UpdateCalls { get; private set; }
    public int DeleteCalls { get; private set; }

    public bool Inserir(Produto produto)
    {
        InsertCalls++;
        produto.Id = _produtos.Count == 0 ? 1 : _produtos.Max(item => item.Id) + 1;
        _produtos.Add(Clone(produto));
        return true;
    }

    public IReadOnlyList<Produto> Listar() => _produtos.Select(Clone).ToList();

    public Produto? BuscarPorId(int id)
    {
        Produto? produto = _produtos.SingleOrDefault(item => item.Id == id);
        return produto is null ? null : Clone(produto);
    }

    public bool Atualizar(Produto produto)
    {
        UpdateCalls++;
        int index = _produtos.FindIndex(item => item.Id == produto.Id);
        if (index < 0)
            return false;

        _produtos[index] = Clone(produto);
        return true;
    }

    public bool Excluir(int id)
    {
        DeleteCalls++;
        return _produtos.RemoveAll(item => item.Id == id) == 1;
    }

    private static Produto Clone(Produto produto) => new()
    {
        Id = produto.Id,
        Nome = produto.Nome,
        Preco = produto.Preco,
        Estoque = produto.Estoque,
        Categoria = produto.Categoria
    };
}
