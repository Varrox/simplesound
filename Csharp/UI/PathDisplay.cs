using Godot;

public partial class PathDisplay : Control {
    [Export] Label path_display;
    [Export] public Button delete;
    [Export] public TextureRect path_type_texture_rect;
    [Export] public bool is_file;

    public string path;

    public override void _Ready() {
        delete.Icon = ApplicationManager.GetThemeIcon("x_icon");

        Color pressed_color = ApplicationManager.GetThemeColor("icon_pressed_color");

        delete.AddThemeColorOverride("icon_pressed_color", pressed_color);
        delete.AddThemeColorOverride("icon_hover_pressed_color", pressed_color);

        delete.AddThemeColorOverride("icon_hover_color", ApplicationManager.GetThemeColor("icon_hover_color"));

        delete.Hide();

        path_type_texture_rect.Texture = ApplicationManager.GetThemeIcon(is_file ? "file_icon" : "folder_icon");
    }
    
    public void SetPath(string _path = null) {
        string text = _path != null ? _path : path;

        path_display.Text = text;
        TooltipText = text;

        if(path_display.Text == string.Empty) delete.Hide();
        else delete.Show();
    }
}
