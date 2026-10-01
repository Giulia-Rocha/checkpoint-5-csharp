using System.IO;
using System.Text;

namespace CadastroProdutos.Wpf.Services;

public sealed class FileOperationLogger : IOperationLogger
{
    private readonly string _filePath;
    private readonly object _syncRoot = new();

    public FileOperationLogger(string filePath)
    {
        _filePath = filePath;
    }

    public void Info(string message) => Write("INFO", message);

    public void Error(string message, Exception exception) =>
        Write("ERRO", $"{message} | {exception.GetType().Name}: {exception.Message}");

    private void Write(string level, string message)
    {
        string? directory = Path.GetDirectoryName(_filePath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }

        string line = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} [{level}] {message}{Environment.NewLine}";
        lock (_syncRoot)
        {
            File.AppendAllText(_filePath, line, Encoding.UTF8);
        }
    }
}
