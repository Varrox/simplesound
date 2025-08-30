using Godot;
using System.IO;

public class Debug
{
    public static void Log(object message)
    {
        GD.Print(message);
    }

    public static void ErrorLog(object message)
    {
        GD.PushError(message);
    }

    public static void LogInvalidCode(LineAttributes line, string code)
    {
        ErrorLog($"{line.ToString()} \'{code}\' is not in valid syntax, and cannot be parsed");
    }
}

public struct LineAttributes
{
    public string path;
    public int line;

    public LineAttributes(string path, int line)
    {
        this.path = path;
        this.line = line;
    }

    public readonly override string ToString()
    {
        return $"{Path.GetFileName(path)}:{line}";
    }
}
