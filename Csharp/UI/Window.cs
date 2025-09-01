using Godot;
using System;

public partial class EditorWindow : Window
{
    public Action OnClose;
    public bool interrupted, cancelled;

    public virtual bool interrupt()
    {
        return true;
    }
}