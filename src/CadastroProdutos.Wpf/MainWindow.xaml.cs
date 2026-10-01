using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Controls;
using CadastroProdutos.Wpf.Data;
using CadastroProdutos.Wpf.Models;
using CadastroProdutos.Wpf.Services;

namespace CadastroProdutos.Wpf;

public partial class MainWindow : Window
{
    private readonly ProdutoService _service;

    public MainWindow(ProdutoService service)
    {
        InitializeComponent();
        _service = service;
        Loaded += (_, _) => ListarProdutos();
    }

    private void Inserir_Click(object sender, RoutedEventArgs e)
    {
        ExecuteSafely(() =>
        {
            if (!TryReadProduct(requireId: false, out Produto produto, out string error))
            {
                ShowValidation(error);
                return;
            }

            OperationResult result = _service.Inserir(produto);
            ShowResult(result);
            if (result.Success)
            {
                LimparFormulario();
                ListarProdutos();
            }
        });
    }

    private void Listar_Click(object sender, RoutedEventArgs e) => ExecuteSafely(ListarProdutos);

    private void Buscar_Click(object sender, RoutedEventArgs e)
    {
        ExecuteSafely(() =>
        {
            if (!TryReadId(out int id))
            {
                ShowValidation("Informe um ID inteiro maior que zero.");
                return;
            }

            Produto? produto = _service.BuscarPorId(id);
            if (produto is null)
            {
                ShowValidation("Produto não encontrado.");
                return;
            }

            FillForm(produto);
            ProdutosDataGrid.ItemsSource = new[] { produto };
            SetStatus($"Produto de ID {id} encontrado.");
        });
    }

    private void Atualizar_Click(object sender, RoutedEventArgs e)
    {
        ExecuteSafely(() =>
        {
            if (!TryReadProduct(requireId: true, out Produto produto, out string error))
            {
                ShowValidation(error);
                return;
            }

            OperationResult result = _service.Atualizar(produto);
            ShowResult(result);
            if (result.Success)
                ListarProdutos();
        });
    }

    private void Excluir_Click(object sender, RoutedEventArgs e)
    {
        ExecuteSafely(() =>
        {
            if (!TryReadId(out int id))
            {
                ShowValidation("Informe um ID inteiro maior que zero.");
                return;
            }

            MessageBoxResult confirmation = MessageBox.Show(
                $"Deseja realmente excluir o produto de ID {id}?",
                "Confirmar exclusão",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirmation != MessageBoxResult.Yes)
                return;

            OperationResult result = _service.Excluir(id);
            ShowResult(result);
            if (result.Success)
            {
                LimparFormulario();
                ListarProdutos();
            }
        });
    }

    private void Limpar_Click(object sender, RoutedEventArgs e)
    {
        LimparFormulario();
        SetStatus("Formulário limpo.");
    }

    private void Sair_Click(object sender, RoutedEventArgs e) => Close();

    private void ProdutosDataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (ProdutosDataGrid.SelectedItem is Produto produto)
            FillForm(produto);
    }

    private void ListarProdutos()
    {
        IReadOnlyList<Produto> produtos = _service.Listar();
        ProdutosDataGrid.ItemsSource = produtos;
        SetStatus($"{produtos.Count} produto(s) listado(s).");
    }

    private bool TryReadProduct(bool requireId, out Produto produto, out string error)
    {
        produto = new Produto();
        var errors = new List<string>();

        int requiredId = 0;
        if (requireId && !TryReadId(out requiredId))
            errors.Add("Informe um ID inteiro maior que zero.");
        else if (requireId)
            produto.Id = requiredId;

        string priceText = PrecoTextBox.Text.Trim();
        bool validPrice = decimal.TryParse(priceText, NumberStyles.Number, CultureInfo.CurrentCulture, out decimal price)
            || decimal.TryParse(priceText, NumberStyles.Number, CultureInfo.InvariantCulture, out price);
        if (!validPrice)
            errors.Add("Informe um preço válido.");

        if (!int.TryParse(EstoqueTextBox.Text.Trim(), out int stock))
            errors.Add("Informe um estoque inteiro válido.");

        produto.Nome = NomeTextBox.Text.Trim();
        produto.Preco = price;
        produto.Estoque = stock;
        produto.Categoria = CategoriaTextBox.Text.Trim();

        errors.AddRange(ProdutoValidator.Validate(produto, requireId));
        error = string.Join(Environment.NewLine, errors.Distinct());
        return errors.Count == 0;
    }

    private bool TryReadId(out int id) =>
        int.TryParse(IdTextBox.Text.Trim(), out id) && id > 0;

    private void FillForm(Produto produto)
    {
        IdTextBox.Text = produto.Id.ToString(CultureInfo.CurrentCulture);
        NomeTextBox.Text = produto.Nome;
        PrecoTextBox.Text = produto.Preco.ToString("N2", CultureInfo.CurrentCulture);
        EstoqueTextBox.Text = produto.Estoque.ToString(CultureInfo.CurrentCulture);
        CategoriaTextBox.Text = produto.Categoria;
    }

    private void LimparFormulario()
    {
        IdTextBox.Clear();
        NomeTextBox.Clear();
        PrecoTextBox.Clear();
        EstoqueTextBox.Clear();
        CategoriaTextBox.Clear();
        ProdutosDataGrid.SelectedItem = null;
        NomeTextBox.Focus();
    }

    private void ShowResult(OperationResult result)
    {
        SetStatus(result.Message);
        MessageBox.Show(
            result.Message,
            result.Success ? "Sucesso" : "Atenção",
            MessageBoxButton.OK,
            result.Success ? MessageBoxImage.Information : MessageBoxImage.Warning);
    }

    private void ShowValidation(string message)
    {
        SetStatus(message.Replace(Environment.NewLine, " "));
        MessageBox.Show(message, "Dados inválidos", MessageBoxButton.OK, MessageBoxImage.Warning);
    }

    private void ExecuteSafely(Action action)
    {
        try
        {
            action();
        }
        catch (DatabaseOperationException exception)
        {
            SetStatus(exception.Message);
            MessageBox.Show(exception.Message, "Erro no banco de dados", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        catch (IOException exception)
        {
            SetStatus("Não foi possível registrar a operação no arquivo de log.");
            MessageBox.Show(exception.Message, "Erro de arquivo", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private void SetStatus(string message) => StatusTextBlock.Text = message;
}
