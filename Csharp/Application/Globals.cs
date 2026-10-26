using Godot;

[GlobalClass]
public partial class Globals : Node {
    public static Globals self;

    [ExportGroup("Nodes")]

    [Export] private Main _main{set{ main = value; }get{ return main; }}
    public static Main main;

    [Export] private Player _player{set{ player = value; }get{ return player; }}
    public static Player player;

    [Export] private Discord _discord{set{ discord = value; }get{ return discord; }}
    public static Discord discord;

    [ExportGroup("Windows")]

    [Export] private AttributeEditor _attribute_editor{set{ attribute_editor = value; }get{ return attribute_editor; }}
    public static AttributeEditor attribute_editor;

    [Export] private DownloadWindow _download_window{set{ download_window = value; }get{ return download_window; }}
    public static DownloadWindow download_window;

    [ExportGroup("Packed Scenes")]

    [Export] private PackedScene _path_display{set{ path_display = value; }get{ return path_display; }}
    public static PackedScene path_display;

    [Export] private PackedScene _confirmation_window{set{ confirmation_window = value; }get{ return confirmation_window; }}
    public static PackedScene confirmation_window;

    public static SaveData save_data;

    Globals() {
        self = this;
    }
}
