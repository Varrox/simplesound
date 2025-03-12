using Godot;

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

    public static void LogInvalidCode(LineAttributes line,string code)
    {
        ErrorLog($"{line} \'{code}\' is not in valid syntax, and cannot be parsed");
    }
}

public struct LineAttributes
{
    public string path;
    public int line, column;

    public LineAttributes(string path, int line, int column)
    {
        this.path = path;
        this.line = line;
        this.column = column;
    }

    public static implicit operator string(LineAttributes attributes)
    {
        return $"{attributes.path} {attributes.line}:{attributes.column}";
    }
}
