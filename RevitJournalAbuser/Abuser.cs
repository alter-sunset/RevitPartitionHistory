using System;
using System.Diagnostics;
using System.IO;

namespace RevitJournalAbuser;

/// <summary>
/// Class to run Revit Journal File in specified Revit version
/// </summary>
public class Abuser: IDisposable
{
    private Process _process;
    
    private int Version
    {
        get;
        set
        {
            if (value is < 2016 or > 2027)
            {
                throw new ArgumentOutOfRangeException(nameof(Version));
            }
            field = value;
        }
    }

    private string ScriptPath
    {
        get;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(ScriptPath));
            }
            if (!File.Exists(value))
            {
                throw new FileNotFoundException("File not found");
            }
            field = value;
        }
    }

    private string RevitExePath
    {
        get;
        set
        {
            if (string.IsNullOrWhiteSpace(value))
            {
                throw new ArgumentNullException(nameof(RevitExePath));
            }
            if (Path.GetExtension(value) != ".exe")
            {
                throw new ArgumentException("Incorrect file extension");
            }
            if (!File.Exists(value))
            {
                throw new FileNotFoundException("Revit.exe not found");
            }
            field = value;
        }
    }

    private TempDirectory TempDirectory
    {
        get;
        set => field = value ?? throw new ArgumentNullException(nameof(TempDirectory));
    }

    /// <summary>
    /// Create new instance of Abuser that will run Autodesk Revit with specified Journal
    /// </summary>
    /// <param name="version">Autodesk Revit version (2016-2027)</param>
    /// <param name="scriptPath">Path to the Journal file</param>
    public Abuser(int version, string scriptPath)
    {
        Version = version;
        RevitExePath = @$"C:\Program Files\Autodesk\Revit {Version}\Revit.exe";
        ScriptPath = scriptPath;
    }

    /// <summary>
    /// Create new instance of Abuser that will run Autodesk Revit with specified Journal
    /// </summary>
    /// <param name="version">Autodesk Revit version (2016-2027)</param>
    /// <param name="tempDirectory">TempDirectory that will be used as temporary working environment</param>
    public Abuser(int version, TempDirectory tempDirectory)
    {
        Version = version;
        RevitExePath = @$"C:\Program Files\Autodesk\Revit {Version}\Revit.exe";
        TempDirectory = tempDirectory;
        ScriptPath = TempDirectory.Script;
    }

    /// <summary>
    /// Create new instance of Abuser that will run Autodesk Revit with specified Journal
    /// </summary>
    /// <param name="revitExePath">Path to Revit.exe</param>
    /// <param name="scriptPath">Path to the Journal file</param>
    public Abuser(string revitExePath, string scriptPath)
    {
        RevitExePath = revitExePath;
        ScriptPath = scriptPath;
    }

    /// <summary>
    /// Create new instance of Abuser that will run Autodesk Revit with specified Journal
    /// </summary>
    /// <param name="revitExePath">Path to Revit.exe</param>
    /// <param name="tempDirectory">TempDirectory that will be used as temporary working environment</param>
    public Abuser(string revitExePath, TempDirectory tempDirectory)
    {
        RevitExePath = revitExePath;
        TempDirectory = tempDirectory;
        ScriptPath = TempDirectory.Script;
    }
    
    /// <summary>
    /// Execute Revit with provided Journal file
    /// </summary>
    public void RunJournalScript()
    {
        ProcessStartInfo startInfo = new()
        {
            FileName = RevitExePath,
            Arguments = $"/language ENG \"{ScriptPath}\"",
            UseShellExecute = false
        };

        _process = Process.Start(startInfo);
    }
    
    public void WaitForExit()
    {
        _process?.WaitForExit();
    }

    public void Dispose()
    {
        if (!_process.HasExited) _process.Kill();
        _process.Dispose();
    }
}