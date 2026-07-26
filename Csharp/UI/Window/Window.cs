using Godot;
using System;

public partial class EditorWindow : Window
{
    public Action OnClose;
    public bool interrupted, cancelled;

    public override void _Ready() {
        ApplicationManager.AddWindow(this);

        Hide();
    }

    public override void _ExitTree() {
        ApplicationManager.windows.Remove(this);
    }

    public virtual bool interrupt() {
        return true;
    }
}