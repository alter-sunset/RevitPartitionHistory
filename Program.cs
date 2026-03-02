using System.Diagnostics;
using System.Text;

namespace RevitCollaborationHistory;

class Program
{
    static void Main(string[] args)
    {
        Console.WriteLine("Provide path to the folder with .rvt files:");
        
        string? inputDirectory = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(inputDirectory) || !Directory.Exists(inputDirectory))
        {
            Console.WriteLine("Path is invalid.");
            inputDirectory = Console.ReadLine();
        }
        
        TempDirectory tempDir = TempDirectory.CreateJournalScript(inputDirectory);
        Directory.CreateDirectory(tempDir.ReportsDirectory);
        
        const string revitExePath = @"C:\Program Files\Autodesk\Revit 2023\Revit.exe";

        ProcessStartInfo startInfo = new()
        {
            FileName = revitExePath,
            Arguments = $"/language ENG \"{tempDir.Script}\"",
            UseShellExecute = false
        };

        Process? process = Process.Start(startInfo);

        process?.WaitForExit();
        
        IEnumerable<Report> reports = tempDir.Reports
            .Select(r => new Report(r));
        
        StringBuilder sb = new();
        sb.AppendLine("FileName|TimeStamp|UserName|Comment");
        foreach (Report report in reports)
        {
            sb.AppendLine($"{report.FileName}|{report.TimeStamp}|{report.UserName}|{report.Comment}");
        }
        
        File.WriteAllText(@"C:\Users\AppolonovFS\Downloads\test\test.csv", sb.ToString());

        // Console.ReadLine();
        Directory.Delete(tempDir.Path, true);
    }
}