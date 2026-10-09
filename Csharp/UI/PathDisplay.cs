using Godot;

public partial class PathDisplay : HBoxContainer
{
    [Export] Label path_display;
    [Export] public Button delete;

    public string path;

    public override void _Ready() {
        delete.Icon = ApplicationManager.theme.GetIcon("x_icon", Constants.THEME_TYPE);
    }
    
    public void SetPath(string _path = null) {
        path_display.Text = _path != null ? _path : path;
        TooltipText = _path;
    }
}
