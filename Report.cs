using System.Globalization;

namespace RevitCollaborationHistory;

/// <summary>
/// Class to read Revit Partition History Report and store last synchronization data
/// </summary>
public class Report
{
    public string FileName { get; }
    public DateTime TimeStamp { get; }
    public string UserName { get; }
    public string Comment { get; }
    
    public Report(string path)
    {
        FileName = Path.GetFileNameWithoutExtension(path);
        
        string secondLine = File.ReadLines(path)
            .Skip(1)
            .FirstOrDefault();
        
        string[] columns = secondLine?.Split('\t');

        if (columns is null || columns.Length < 3) return;
        
        TimeStamp = DateTime.ParseExact(
            columns[0],
            "dd/MM/yyyy HH:mm:ss",
            CultureInfo.InvariantCulture);

        UserName = columns[1];
        Comment = columns[2];
    }

    /// <summary>
    /// </summary>
    /// <returns>String formated like this: FileName|TimeStamp|UserName|Comment</returns>
    public override string ToString()
    {
        return $"{FileName}|{TimeStamp}|{UserName}|{Comment}";
    }
    
    public string ToString(string separator)
    {
        return string.Join(separator, FileName, TimeStamp, UserName, Comment);
    }
}