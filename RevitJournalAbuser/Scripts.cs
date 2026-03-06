using System.IO;

namespace RevitJournalAbuser;

/// <summary>
/// Various scripts to use in a Journal file
/// </summary>
public static class Scripts
{
    /// <summary>
    /// Generate script to create report on Partition History of a rvt model
    /// </summary>
    /// <param name="filePath">Path to rvt model file</param>
    /// <param name="outputDirectory">Directory to put report at</param>
    /// <returns>Script in string format</returns>
    public static string CreatePartitionHistory(this string filePath, string outputDirectory)
    {
        string fileName = Path.GetFileName(filePath);
        string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filePath);
        string reportName = $"{fileNameWithoutExtension}.txt";
        string reportPath = Path.Combine(outputDirectory, reportName);
        return  $"""
                 '
                 Jrn.Command "Ribbon"  , "Show usernames, times, and comments for successive saves of a file. , ID_PARTITIONS_SHOW_HISTORY" 
                 Jrn.Data  _
                           "FileDialog"  , "IDOK" , "{filePath}"  _
                           , "rvt" , "{fileName}" , "{fileNameWithoutExtension}" 
                 Jrn.Data  _
                           "FileType"  , "RVT Files (*.rvt)" 
                 Jrn.PushButton "Modal , History , Dialog_Revit_PartitionsHistory"  _
                               , "Export..., Control_Revit_FileExport" 
                 Jrn.Data  _
                             "FileDialog"  , "IDOK" , "{reportPath}"  _
                             , "txt" , "{reportName}" , "" 
                 Jrn.Data  _
                             "FileType"  , "Delimited text (*.txt)" 
                 Jrn.PushButton "Modal , History , Dialog_Revit_PartitionsHistory"  _
                               , "Close, IDCANCEL"
                 """;
    }

    /// <summary>
    /// Generate script to execute an ExternalCommand (e.g. custom addon)
    /// </summary>
    /// <param name="tabName">Name of a Tab on a Ribbon</param>
    /// <param name="panelName">Name of a Panel in the Tab</param>
    /// <param name="buttonName">Name of a Button in the Panel</param>
    /// <param name="fullPathToCommand">Full path to Class associated with the Button</param>
    /// <returns>Script is string format</returns>
    public static string RunExternalCommandButton(string tabName, string panelName, string buttonName, string fullPathToCommand)
    {
        buttonName = buttonName.Split('\n')[0]; // Have to use only first line of a name
        return $"""
                Jrn.RibbonEvent "Execute external command:CustomCtrl_%CustomCtrl_%{tabName}%{panelName}%{buttonName}:{fullPathToCommand}"
                """;
    }
}