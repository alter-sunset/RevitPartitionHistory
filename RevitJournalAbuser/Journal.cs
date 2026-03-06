using System;
using System.IO;
using System.Text;

namespace RevitJournalAbuser;

public class Journal: IDisposable
{
    private const string Header = """
                                  ' 
                                  Dim Jrn
                                  Set Jrn = CrsJournalScript
                                  """;

    private const string Footer = """
                                  '
                                  Jrn.Command "Internal"  , "Quit the application; prompts to save projects , ID_APP_EXIT"
                                  """;

    private readonly string _scriptFilePath;
    private readonly StringBuilder _stringBuilder;
    private bool _completed;
    
    public Journal(string scriptPath, params string[] parts)
    {
        _scriptFilePath = scriptPath;
        _stringBuilder = new StringBuilder(Header);
        foreach (string part in parts)
        {
            _stringBuilder.AppendLine(part);
        }
    }

    public void AddScriptParts(params string[] parts)
    {
        foreach (string part in parts)
        {
            _stringBuilder.AppendLine(part);
        }
    }

    public void Complete()
    {
        _stringBuilder.AppendLine(Footer);
        _completed = true;
        File.WriteAllText(_scriptFilePath, _stringBuilder.ToString());
        _stringBuilder.Clear();
    }

    public void CompleteWithoutExit()
    {
        _completed = true;
        File.WriteAllText(_scriptFilePath, _stringBuilder.ToString());
        _stringBuilder.Clear();
    }
    
    public void Dispose()
    {
        if (!_completed)
        {
            Complete();
        }
    }
}