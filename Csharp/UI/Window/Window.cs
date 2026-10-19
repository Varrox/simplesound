using Godot;

public partial class EditorWindow : Window {
    [Signal]
    public delegate void OnCloseEventHandler();
    public bool interrupted, cancelled;

    public override void _Ready() {
        ApplicationManager.AddWindow(this);

        Theme = ApplicationManager.theme;

        Hide();
    }

    public override void _ExitTree() {
        ApplicationManager.windows.Remove(this);
    }

    public virtual bool interrupt() {
        return true;
    }
}