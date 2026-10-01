using CadastroProdutos.Wpf.Models;

namespace CadastroProdutos.Wpf.Data;

public interface IProdutoRepository
{
    bool Inserir(Produto produto);
    IReadOnlyList<Produto> Listar();
    Produto? BuscarPorId(int id);
    bool Atualizar(Produto produto);
    bool Excluir(int id);
}
