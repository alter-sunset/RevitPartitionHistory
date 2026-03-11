using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace RevitPartitionHistory;

internal static class Program
{
    private static readonly string DownloadsFolderPath =
        Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile), "Downloads");

    private static void Main()
    {
        string csvPath = Path.Combine(DownloadsFolderPath, $"{DateTime.Now:yyyy-MM-dd-HH-mm-ss}.txt");
        string inputDir = ObtainInputDirectory();
        
        IEnumerable<string> files = Directory.EnumerateFiles(inputDir, "*.rvt", SearchOption.AllDirectories);

        using (ReportsTempDirectory reportsTempDir = new(files))
        {
            IEnumerable<Report> reports = reportsTempDir.GetReports(2023);
            PrintResult(reports, csvPath);
        }
        Finalize(csvPath);
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

    private static void Finalize(string csvPath)
    {
        Console.WriteLine("The work is done! Final report available at " + csvPath);
        Console.WriteLine("Press Enter to exit...");
        Console.ReadLine();
    }
}