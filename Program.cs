using System.Diagnostics;
using System.Text;

namespace RevitCollaborationHistory;

class Program
{
    private static readonly string DownloadsFolderPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");
    
    static void Main(string[] args)
    {
        string inputDirectory = ObtainInputDirectory();
        
        using TempDirectory tempDir = CreateJournalScript(inputDirectory);
        
        IEnumerable<Report> reports = GetReports(tempDir);
        string csvPath = Path.Combine(DownloadsFolderPath, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.csv");
        
        PrintResult(reports, csvPath);
    }

    /// <summary>
    /// Read input directory from console
    /// </summary>
    /// <returns>Directory</returns>
    private static string ObtainInputDirectory()
    {
        Console.WriteLine("Provide path to the folder with .rvt files:");
        
        string? inputDirectory = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(inputDirectory) || !Directory.Exists(inputDirectory))
        {
            Console.WriteLine("Path is invalid.");
            inputDirectory = Console.ReadLine();
        }
        return inputDirectory;
    }
    
    /// <summary>
    /// Creates journal script to export partition history of provided RVT files
    /// </summary>
    /// <param name="inputDirectory">Directory that contains RVT files</param>
    /// <returns>Path to the resulting Journal Script</returns>
    private static TempDirectory CreateJournalScript(string inputDirectory)
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
        Directory.CreateDirectory(tempDir.ReportsDirectory);
        
        return tempDir;
    }

    /// <summary>
    /// Start Revit process with a given Journal Script
    /// </summary>
    /// <param name="tempDir">TempDirectory that contains Journal Script and Reports folder</param>
    private static IEnumerable<Report> GetReports(TempDirectory tempDir)
    {
        const string revitExePath = @"C:\Program Files\Autodesk\Revit 2023\Revit.exe";

        ProcessStartInfo startInfo = new()
        {
            FileName = revitExePath,
            Arguments = $"/language ENG \"{tempDir.Script}\"",
            UseShellExecute = false
        };

        Process? process = Process.Start(startInfo);

        process?.WaitForExit();

        return tempDir.Reports
            .Select(r => new Report(r));
    }

    /// <summary>
    /// Print out results to csv file
    /// </summary>
    /// <param name="reports">Collection of reports to read data from</param>
    private static void PrintResult(IEnumerable<Report> reports, string csvPath)
    {
        StringBuilder sb = new();
        sb.AppendLine("FileName|TimeStamp|UserName|Comment");
        foreach (Report report in reports)
        {
            sb.AppendLine(report.ReportLine);
        }
        
        File.WriteAllText(csvPath, sb.ToString());
    }
}