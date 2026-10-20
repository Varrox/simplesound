using Godot;

public partial class ThemePopupMenu : PopupMenu {
    public override void _Ready() {
        ApplicationManager.ApplyPopupMenuStyle(this);
    }
}