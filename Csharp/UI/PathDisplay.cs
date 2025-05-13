using Godot;

public partial class PathDisplay : Control
{
    [Export] Label path_display;
    [Export] public Button delete;
    public string path;
    public void set_path(string _path = null)
    {
        path_display.Text = _path != null ? _path : path;
    }
}
