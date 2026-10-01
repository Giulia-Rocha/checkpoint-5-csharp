namespace CadastroProdutos.Wpf.Data;

public sealed class DatabaseOperationException : Exception
{
    public DatabaseOperationException(string message, Exception innerException)
        : base(message, innerException)
    {
    }
}
