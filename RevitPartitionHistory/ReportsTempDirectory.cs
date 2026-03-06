using System.Collections.Generic;
using System.IO;
using RevitJournalAbuser;

namespace RevitPartitionHistory;

public class ReportsTempDirectory: TempDirectory
{
    /// <summary>
    /// Path to Reports subdirectory
    /// </summary>
    public string ReportsDirectory => Path.Combine(Dir, "Reports");
    
    /// <summary>
    /// Resulting reports path
    /// </summary>
    public IEnumerable<string> Reports => Directory.EnumerateFiles(ReportsDirectory);

    public ReportsTempDirectory()
    {
        Directory.CreateDirectory(ReportsDirectory);
    }
}