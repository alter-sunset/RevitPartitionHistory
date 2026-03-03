using System;
using System.Collections.Generic;
using System.IO;

namespace RevitCollaborationHistory;

public class TempDirectory : IDisposable
{
    public TempDirectory()
    {
        string tempDir = Path.GetTempPath();
        Dir = Path.Combine(tempDir, "RevitCollaborationHistory");
        Directory.CreateDirectory(Dir);
    }

    private string Dir { get; }
    public string Script => Path.Combine(Dir, "script.txt");
    
    public string ReportsDirectory => Path.Combine(Dir, "Reports");
    public IEnumerable<string> Reports => Directory.EnumerateFiles(ReportsDirectory);

    public void Dispose()
    {
        Directory.Delete(Dir, true);
    }
}