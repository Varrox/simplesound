using Godot;

public partial class EditorWindow : Window
{
    [Signal] public delegate void OnCloseEventHandler();
    bool interrupted;

    public virtual bool interrupt()
    {
        return true;
    }
}