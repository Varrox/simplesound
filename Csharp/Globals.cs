using Godot;

public partial class Globals : Node
{
    // Textures
    [Export] Texture2D _play_texture {set{ play_texture = value; }get { return play_texture; }}
    [Export] Texture2D _pause_texture {set{ pause_texture = value; } get { return pause_texture; }}
    [Export] Texture2D _no_play_texture {set{ no_play_texture = value; }get { return no_play_texture; }}
    [Export] Texture2D _loop_on_texture {set{ loop_on_texture = value; }get { return loop_on_texture; }}
    [Export] Texture2D _loop_off_texture {set{ loop_off_texture = value; }get { return loop_off_texture; }}
    [Export] Texture2D _shuffle_on_texture {set{ shuffle_on_texture = value; }get { return shuffle_on_texture; }}
    [Export] Texture2D _shuffle_off_texture {set{ shuffle_off_texture = value; }get { return shuffle_off_texture; }}
    [Export] Texture2D _mute_texture {set{ mute_texture = value; }get { return mute_texture; }}
    [Export] Texture2D _unmute_texture {set{ unmute_texture = value; }get { return unmute_texture; }}
    [Export] Texture2D _default_cover{set{ default_cover = value; }get { return default_cover; }}
    [Export] Texture2D _default_cover_highres{set{ default_cover_highres = value; }get { return default_cover_highres; }}

    public static Texture2D play_texture, pause_texture, no_play_texture, loop_on_texture, loop_off_texture, shuffle_on_texture, shuffle_off_texture, mute_texture, unmute_texture, default_cover, default_cover_highres;

    [Export] Main _main{set{ main = value; }get{ return main; }}
    public static Main main;

    [Export] Player _player{set{ player = value; }get{ return player; }}
    public static Player player;

    [Export] Discord _discord{set{ discord = value; }get{ return discord; }}
    public static Discord discord;

    [Export] AttributeEditor _attribute_editor{set{ attribute_editor = value; }get{ return attribute_editor; }}
    public static AttributeEditor attribute_editor;

    [Export] FileDialog _file_dialog{set{ file_dialog = value; }get { return file_dialog; }}
    public static FileDialog file_dialog;

    [Export] Node _self{set{ self = value; }get { return self; }}
    public static Node self;

    [Export] Color _highlight{set{ highlight = value; }get{ return highlight; }}
    public static Color highlight;

    [Export] Color _lower_highlight{set{ lower_highlight = value; }get{ return lower_highlight; }}
    public static Color lower_highlight;

    [Export] PackedScene _confirmation_window{set{ confirmation_window = value; }get{ return confirmation_window; }}
    public static PackedScene confirmation_window;

    [Export] ContextMenu _playlist_menu{set{ playlist_menu = value; }get{ return playlist_menu; }}
    public static ContextMenu playlist_menu;

    [Export] ContextMenu _song_menu{set{ song_menu = value; }get{ return song_menu; }}
    public static ContextMenu song_menu;

    public static Vector2I main_window_minimum_size = new Vector2I(850, 350);
    public static SaveData save_data;

    public override void _Ready()
    {
        GetTree().Root.MinSize = main_window_minimum_size;
        GetTree().Root.CloseRequested += OnClose;
    }

    public static void OnClose()
    {
        main.Save();
        Discord.ShutDown();
    }

    public static void ResetFileDialogParameters()
    {
        file_dialog.Filters = null;
        file_dialog.OkButtonText = "";
    }

    public static void SetFileDialogSongs()
    {
        file_dialog.Filters = new[] { "*.mp3", "*.wav", "*.ogg" };
        file_dialog.FileMode = FileDialog.FileModeEnum.OpenFiles;
        file_dialog.OkButtonText = "Import audio files";
        file_dialog.Title = "Select Audio files";
    }

    public static void SetFileDialogFile()
    {
        file_dialog.Filters = null;
        file_dialog.FileMode = FileDialog.FileModeEnum.OpenFile;
        file_dialog.OkButtonText = "Select file";
        file_dialog.Title = "Select file";
    }

    public static void SetFileDialogCover()
    {
        file_dialog.Filters = new[] { "*.jpeg", "*.jpg", "*.png", "*.webp" };
        file_dialog.FileMode = FileDialog.FileModeEnum.OpenFile;
        file_dialog.OkButtonText = "Import Cover";
        file_dialog.Title = "Select Cover";
    }

    public static void SetFileDialogPlaylist()
    {
        file_dialog.Filters = new[] { "*.json"};
        file_dialog.FileMode = FileDialog.FileModeEnum.OpenFile;
        file_dialog.OkButtonText = "Import Playlist";
        file_dialog.Title = "Select Playlist";
    }
}
