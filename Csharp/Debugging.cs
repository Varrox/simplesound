using Godot;

class Debug
{
    public static void Log(object message)
    {
        GD.Print(message);
    }

    public static void ErrorLog(object message)
    {
        GD.PushError(message);
    }
}
