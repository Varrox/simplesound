using Godot;

public partial class ThemeWindow : Window {
    [Signal]
    public delegate void OnCloseEventHandler();

    public override void _EnterTree() {
        ApplicationManager.AddWindow(this);

        Theme = ApplicationManager.theme;
    }

    public override void _ExitTree() {
        ApplicationManager.windows.Remove(this);
    }
}