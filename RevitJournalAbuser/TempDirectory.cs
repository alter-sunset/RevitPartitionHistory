using System;
using System.IO;

namespace RevitJournalAbuser;

/// <summary>
/// Temp directory that will contain Journal Script, temp journals and other temp variables
/// </summary>
public abstract class TempDirectory : IDisposable
{
    /// <summary>
    /// Base temp directory
    /// </summary>
    protected string Dir { get; }
    
    /// <summary>
    /// Path to Journal Script file
    /// </summary>
    public string Script => Path.Combine(Dir, "script.txt");

    protected TempDirectory()
    {
        string tempDir = Path.GetTempPath();
        Dir = Path.Combine(tempDir, "RevitJournalAbuser");
        Directory.CreateDirectory(Dir);
    }

    /// <summary>
    /// Create SubDirectory inside main temp directory
    /// </summary>
    /// <param name="subDirectoryName">Name of SubDirectory</param>
    /// <returns>Full Path to SubDirectory</returns>
    public string CreateSubDirectory(string subDirectoryName)
    {
        string subDir = Path.Combine(Dir, subDirectoryName);
        Directory.CreateDirectory(subDir);
        return subDir;
    }
    
    /// <summary>
    /// Delete temp Directory and all of its contents when the work is done
    /// </summary>
    public void Dispose()
    {
        Directory.Delete(Dir, true);
    }
}