using Godot;

[GlobalClass]
public partial class PathDisplay : Button {
    TextureRect path_type_texture_rect;
    Label path_display;
    public Button delete;
    public bool is_file;

    public string path {
        get {
            return field;
        }
        set {
            field = value;

            path_display.Text = value;
            TooltipText = value;

            Disabled = path_display.Text == string.Empty;
            delete.Visible = path_display.Text != string.Empty;
        }
    }

    public override void _Ready() {
        HBoxContainer container = new HBoxContainer();
        AddChild(container);

        container.SetAnchorsPreset(LayoutPreset.FullRect);

        // File icon

        MarginContainer margin = new MarginContainer();
        container.AddChild(margin);

        margin.CustomMinimumSize = new Vector2(25, 25);

        margin.AddThemeConstantOverride("margin_left", 3);
        margin.AddThemeConstantOverride("margin_top", 3);
        margin.AddThemeConstantOverride("margin_right", 3);
        margin.AddThemeConstantOverride("margin_bottom", 3);

        path_type_texture_rect = new TextureRect();
        margin.AddChild(path_type_texture_rect);

        path_type_texture_rect.Texture = ApplicationManager.GetThemeIcon(is_file ? "file_icon" : "folder_icon");
        path_type_texture_rect.ExpandMode = TextureRect.ExpandModeEnum.IgnoreSize;

        // Label

        path_display = new Label();
        container.AddChild(path_display);

        path_display.ClipText = true;
        path_display.TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;

        path_display.SizeFlagsHorizontal = SizeFlags.ExpandFill;

        path_display.MouseFilter = MouseFilterEnum.Pass;

        path_display.AddThemeFontSizeOverride("font_size", 11);

        // Delete

        delete = new Button();
        container.AddChild(delete);

        delete.ExpandIcon = true;
        delete.Flat = true;

        delete.Icon = ApplicationManager.GetThemeIcon("x_icon");

        delete.CustomMinimumSize = new Vector2(25, 25);

        Color pressed_color = ApplicationManager.GetThemeColor("icon_pressed_color");

        delete.AddThemeColorOverride("icon_pressed_color", pressed_color);
        delete.AddThemeColorOverride("icon_hover_pressed_color", pressed_color);

        delete.AddThemeColorOverride("icon_hover_color", ApplicationManager.GetThemeColor("icon_hover_color"));

        delete.Hide();

        path = "";
    }

    public override void _Pressed() {
        OS.ShellShowInFileManager(path);
    }
}
