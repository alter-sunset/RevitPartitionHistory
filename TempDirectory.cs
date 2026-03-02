using System.Text;

namespace RevitCollaborationHistory;

public class TempDirectory : IDisposable
{
    public string Path { get; } = Directory.CreateTempSubdirectory().FullName;
    public string Script => System.IO.Path.Combine(Path, "script.txt");
    
    public string ReportsDirectory => System.IO.Path.Combine(Path, "Reports");
    public IEnumerable<string> Reports => Directory.EnumerateFiles(ReportsDirectory);

    public void Dispose()
    {
        Directory.Delete(Path, true);
    }
}