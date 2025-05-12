using Godot;

public partial class PathDisplay : Control
{
    [Export] Label path_display;
    [Export] Button delete;
    public string path;
    public void set_path()
    {
        path_display.Text = path;
    }
}
