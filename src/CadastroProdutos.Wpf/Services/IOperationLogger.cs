namespace CadastroProdutos.Wpf.Services;

public interface IOperationLogger
{
    void Info(string message);
    void Error(string message, Exception exception);
}
