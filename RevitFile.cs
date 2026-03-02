namespace RevitCollaborationHistory;

public class RevitFile(string filePath, string outputDirectory)
{
    private const string Txt = ".txt";
    private string FilePath { get; } = filePath;
    private string OutputDirectory { get; } = outputDirectory;

    private string FileName => Path.GetFileName(FilePath);
    private string FileNameWithoutExtension => Path.GetFileNameWithoutExtension(FilePath);

    private string ReportName => $"{FileNameWithoutExtension}{Txt}";
    private string ReportPath => Path.Combine(OutputDirectory, ReportName);

    public string Script => $"""
                             '
                             Jrn.Command "Ribbon"  , "Show usernames, times, and comments for successive saves of a file. , ID_PARTITIONS_SHOW_HISTORY" 
                             Jrn.Data  _
                                       "FileDialog"  , "IDOK" , "{FilePath}"  _
                                       , "rvt" , "{FileName}" , "{FileNameWithoutExtension}" 
                             Jrn.Data  _
                                       "FileType"  , "RVT Files (*.rvt)" 
                             Jrn.PushButton "Modal , History , Dialog_Revit_PartitionsHistory"  _
                                           , "Export..., Control_Revit_FileExport" 
                             Jrn.Data  _
                                         "FileDialog"  , "IDOK" , "{ReportPath}"  _
                                         , "txt" , "{ReportName}" , "" 
                             Jrn.Data  _
                                         "FileType"  , "Delimited text (*.txt)" 
                             Jrn.PushButton "Modal , History , Dialog_Revit_PartitionsHistory"  _
                                           , "Close, IDCANCEL"
                             """;
}