using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using RevitJournalAbuser;

namespace RevitPartitionHistory;

internal static class Program
{
    private static readonly string DownloadsFolderPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

    private static void Main()
    {
        string csvPath = Path.Combine(DownloadsFolderPath, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt");
        string inputDir = ObtainInputDirectory();
        
        IEnumerable<string> files = Directory
            .EnumerateFiles(inputDir, "*.rvt", SearchOption.AllDirectories)
            .Where(File.Exists)
            .Where(f => Path.GetExtension(f) == ".rvt");

        using (ReportsTempDirectory reportsTempDir = CreateTempEnvironment(files))
        {
            IEnumerable<Report> reports = GetReports(reportsTempDir, 2023);
            PrintResult(reports, csvPath);
        }
        Finisher(csvPath);
    }

    /// <summary>
    /// Read input directory from console
    /// </summary>
    /// <returns>Directory</returns>
    private static string ObtainInputDirectory()
    {
        Console.WriteLine("Provide path to the folder with .rvt files:");
        
        string inputDir = Console.ReadLine();
        while (string.IsNullOrWhiteSpace(inputDir) || !Directory.Exists(inputDir))
        {
            Console.WriteLine("Path is invalid.");
            inputDir = Console.ReadLine();
        }
        return inputDir;
    }

    /// <summary>
    /// Creates temp environment with journal script to export partition history of provided RVT files
    /// </summary>
    /// <param name="files">RVT files path</param>
    /// <returns>Path to the resulting temp environment</returns>
    private static ReportsTempDirectory CreateTempEnvironment(IEnumerable<string> files)
    {
        ReportsTempDirectory reportsTempDir = new();
        
        string[] fileScripts = files
            .Select(file => file.CreatePartitionHistory(reportsTempDir.ReportsDirectory))
            .ToArray();
        
        using Journal journal = new(reportsTempDir.Script, fileScripts);
        
        return reportsTempDir;
    }

    /// <summary>
    /// Start Revit process with a given Journal Script
    /// </summary>
    /// <param name="reportsTempDir">TempDirectory that contains Journal Script and Reports folder</param>
    /// <param name="version">Revit version (eg 2022)</param>
    private static IEnumerable<Report> GetReports(ReportsTempDirectory reportsTempDir, int version)
    {
        Abuser abuser = new(version, reportsTempDir);
        
        abuser.RunJournalScript();

        return reportsTempDir.Reports
            .Select(r => new Report(r));
    }

    /// <summary>
    /// Print out results to csv file
    /// </summary>
    /// <param name="reports">Collection of reports to read data from</param>
    /// <param name="csvPath">Path to the resulting csv</param>
    private static void PrintResult(IEnumerable<Report> reports, string csvPath)
    {
        StringBuilder sb = new();
        sb.AppendLine("FileName|TimeStamp|UserName|Comment");
        foreach (Report report in reports)
        {
            sb.AppendLine(report.ToString());
        }
        
        File.WriteAllText(csvPath, sb.ToString());
    }

    private static void Finisher(string csvPath)
    {
        Console.WriteLine("The work is done! Final report available at " + csvPath);
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }
}