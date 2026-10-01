using System.IO;
using System.Windows;
using CadastroProdutos.Wpf.Data;
using CadastroProdutos.Wpf.Services;
using Microsoft.Extensions.Configuration;

namespace CadastroProdutos.Wpf;

public partial class App : Application
{
    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        try
        {
            IConfiguration configuration = new ConfigurationBuilder()
                .SetBasePath(AppContext.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false)
                .Build();

            string connectionString = configuration["ConnectionStrings:ProdutosDatabase"]
                ?? throw new InvalidOperationException("Connection string não configurada.");

            var logger = new FileOperationLogger(
                Path.Combine(AppContext.BaseDirectory, "logs", "operacoes.log"));
            IProdutoRepository repository = new ProdutoRepository(connectionString, logger);
            var service = new ProdutoService(repository);

            MainWindow = new MainWindow(service);
            MainWindow.Show();
        }
        catch (Exception exception)
        {
            MessageBox.Show(
                $"Não foi possível iniciar a aplicação.\n\n{exception.Message}",
                "Erro de inicialização",
                MessageBoxButton.OK,
                MessageBoxImage.Error);
            Shutdown(1);
        }
    }
}
