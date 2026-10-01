using CadastroProdutos.Wpf.Data;
using CadastroProdutos.Wpf.Models;

namespace CadastroProdutos.Wpf.Services;

public sealed class ProdutoService
{
    private readonly IProdutoRepository _repository;

    public ProdutoService(IProdutoRepository repository)
    {
        _repository = repository;
    }

    public OperationResult Inserir(Produto produto)
    {
        OperationResult? invalidResult = Validate(produto);
        if (invalidResult is not null)
            return invalidResult;

        return _repository.Inserir(produto)
            ? OperationResult.Ok("Produto inserido com sucesso.")
            : OperationResult.Fail("Nenhum produto foi inserido.");
    }

    public IReadOnlyList<Produto> Listar() => _repository.Listar();

    public Produto? BuscarPorId(int id) => id > 0 ? _repository.BuscarPorId(id) : null;

    public OperationResult Atualizar(Produto produto)
    {
        OperationResult? invalidResult = Validate(produto, requireId: true);
        if (invalidResult is not null)
            return invalidResult;

        return _repository.Atualizar(produto)
            ? OperationResult.Ok("Produto atualizado com sucesso.")
            : OperationResult.Fail("Produto não encontrado para atualização.");
    }

    public OperationResult Excluir(int id)
    {
        if (id <= 0)
            return OperationResult.Fail("Informe um ID válido.");

        return _repository.Excluir(id)
            ? OperationResult.Ok("Produto excluído com sucesso.")
            : OperationResult.Fail("Produto não encontrado para exclusão.");
    }

    private static OperationResult? Validate(Produto produto, bool requireId = false)
    {
        IReadOnlyList<string> errors = ProdutoValidator.Validate(produto, requireId);
        return errors.Count == 0 ? null : OperationResult.Fail(string.Join(Environment.NewLine, errors));
    }
}
