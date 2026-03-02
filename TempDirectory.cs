using System.Text;

namespace RevitCollaborationHistory;

public class TempDirectory
{
    public string Path { get; } = Directory.CreateTempSubdirectory().FullName;
    public string Script => System.IO.Path.Combine(Path, "script.txt");
    
    public string ReportsDirectory => System.IO.Path.Combine(Path, "Reports");
    public IEnumerable<string> Reports => Directory.EnumerateFiles(ReportsDirectory);
    
    /// <summary>
    /// Creates journal script to export partition history of provided RVT files
    /// </summary>
    /// <param name="inputDirectory">Directory that contains RVT files</param>
    /// <returns>Path to the resulting Journal Script</returns>
    public static TempDirectory CreateJournalScript(string inputDirectory)
    {
        const string header = """
                              ' 
                              Dim Jrn
                              Set Jrn = CrsJournalScript
                              """;

        const string footer = """
                              '
                              Jrn.Command "Internal"  , " , ID_APP_EXIT"
                              """;

        TempDirectory tempDir = new();
        
        IEnumerable<string> fileScripts = Directory.EnumerateFiles(inputDirectory, "*.rvt", SearchOption.AllDirectories)
            .Where(File.Exists)
            .Where(f => System.IO.Path.GetExtension(f) == ".rvt")
            .Select(f => new RevitFile(f, tempDir.ReportsDirectory))
            .Select(rF => rF.Script);
        
        StringBuilder sb = new();
        sb.AppendLine(header);
        foreach (string fileScript in fileScripts)
        {
            sb.AppendLine(fileScript);
        }
        sb.AppendLine(footer);
        
        File.WriteAllText(tempDir.Script, sb.ToString());
        
        return tempDir;
    }
}