using Godot;

public partial class EditorWindow : Window
{
    [Signal] public delegate void OnCloseEventHandler();
    public bool interrupted, cancelled;

    public virtual bool interrupt()
    {
        return true;
    }
}