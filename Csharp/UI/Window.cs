using Godot;

public partial class EditorWindow : Window
{
    [Signal] public delegate void OnCloseEventHandler();
}