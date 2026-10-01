using CadastroProdutos.Wpf.Models;

namespace CadastroProdutos.Wpf.Services;

public static class ProdutoValidator
{
    public static IReadOnlyList<string> Validate(Produto produto, bool requireId = false)
    {
        var errors = new List<string>();

        if (requireId && produto.Id <= 0)
            errors.Add("Informe um ID válido.");
        if (string.IsNullOrWhiteSpace(produto.Nome))
            errors.Add("O nome é obrigatório.");
        else if (produto.Nome.Length > 120)
            errors.Add("O nome deve ter no máximo 120 caracteres.");
        if (produto.Preco < 0)
            errors.Add("O preço não pode ser negativo.");
        if (produto.Estoque < 0)
            errors.Add("O estoque não pode ser negativo.");
        if (string.IsNullOrWhiteSpace(produto.Categoria))
            errors.Add("A categoria é obrigatória.");
        else if (produto.Categoria.Length > 80)
            errors.Add("A categoria deve ter no máximo 80 caracteres.");

        return errors;
    }
}
