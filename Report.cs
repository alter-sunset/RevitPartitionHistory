using System.Globalization;

namespace RevitCollaborationHistory;

public class Report
{
    public string FileName { get; }
    public DateTime TimeStamp { get; }
    public string? UserName { get; }
    public string? Comment { get; }
    public string ReportLine => $"{FileName}|{TimeStamp}|{UserName}|{Comment}";
    
    public Report(string path)
    {
        FileName = Path.GetFileNameWithoutExtension(path);
        
        string? secondLine = File.ReadLines(path)
            .Skip(1)
            .FirstOrDefault();
        
        string[]? parts = secondLine?.Split('\t');

        if (parts is null || parts.Length < 3) return;
        
        TimeStamp = DateTime.ParseExact(
            parts[0],
            "dd/MM/yyyy HH:mm:ss",
            CultureInfo.InvariantCulture);

        UserName = parts[1];
        Comment = parts[2];
    }
}