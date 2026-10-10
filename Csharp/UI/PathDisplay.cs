using Godot;

public partial class PathDisplay : Control
{
    [Export] Label path_display;
    [Export] public Button delete;
    [Export] public TextureRect path_type_texture_rect;
    [Export] public bool is_file;

    public string path;

    public override void _Ready() {
        delete.Icon = ApplicationManager.theme.GetIcon("x_icon", Constants.THEME_TYPE);

        Color pressed_color = ApplicationManager.theme.GetColor("icon_pressed_color", Constants.THEME_TYPE);

        delete.AddThemeColorOverride("icon_pressed_color", pressed_color);
        delete.AddThemeColorOverride("icon_hover_pressed_color", pressed_color);

        delete.AddThemeColorOverride("icon_hover_color", ApplicationManager.theme.GetColor("icon_hover_color", Constants.THEME_TYPE));

        delete.Hide();

        path_type_texture_rect.Texture = ApplicationManager.theme.GetIcon(is_file ? "file_icon" : "folder_icon", Constants.THEME_TYPE);
    }
    
    public void SetPath(string _path = null) {
        string text = _path != null ? _path : path;

        path_display.Text = text;
        TooltipText = text;

        if(path_display.Text == string.Empty) delete.Hide();
        else delete.Show();
    }
}
