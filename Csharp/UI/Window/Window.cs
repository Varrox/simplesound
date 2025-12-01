using Godot;
using System;

public partial class EditorWindow : Window
{
    public Action OnClose;
    public bool interrupted, cancelled;

    public override void _Ready()
    {
        Hide();
    }

    public virtual bool interrupt()
    {
        return true;
    }
}