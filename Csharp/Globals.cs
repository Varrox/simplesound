using Godot;
using System;

[GlobalClass]
public partial class Globals : Node
{
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

    [Export] private FileDialog _file_dialog{set{ file_dialog = value; }get { return file_dialog; }}
    public static FileDialog file_dialog;

    [Export] private ContextMenu _playlist_menu{set{ playlist_menu = value; }get{ return playlist_menu; }}
    public static ContextMenu playlist_menu;

    [Export] private ContextMenu _track_menu{set{ track_menu = value; }get{ return track_menu; }}
    public static ContextMenu track_menu;

    [ExportGroup("Packed Scenes")]

    [Export] private PackedScene _path_display{set{ path_display = value; }get{ return path_display; }}
    public static PackedScene path_display;

    [Export] private PackedScene _confirmation_window{set{ confirmation_window = value; }get{ return confirmation_window; }}
    public static PackedScene confirmation_window;

    public static SaveData save_data;

    Globals() {
        self = this;
    }

    public static void ResetFileDialogParameters() {
        file_dialog.Filters = null;
        file_dialog.OkButtonText = "";
    }

    public static void SetFileDialogTracks() {
        file_dialog.Filters = new[] { "*.mp3", "*.wav", "*.ogg" };
        file_dialog.FileMode = FileDialog.FileModeEnum.OpenFiles;
        file_dialog.OkButtonText = "Import audio files";
        file_dialog.Title = "Select Audio files";
    }

    public static void SetFileDialogFile() {
        file_dialog.Filters = null;
        file_dialog.FileMode = FileDialog.FileModeEnum.OpenFile;
        file_dialog.OkButtonText = "Select file";
        file_dialog.Title = "Select file";
    }

    public static void SetFileDialogCover() {
        file_dialog.Filters = new[] { "*.jpeg", "*.jpg", "*.png", "*.webp" };
        file_dialog.FileMode = FileDialog.FileModeEnum.OpenFile;
        file_dialog.OkButtonText = "Import Cover";
        file_dialog.Title = "Select Cover";
    }

    public static void SetFileDialogPlaylist() {
        file_dialog.Filters = new[] { "*.json"};
        file_dialog.FileMode = FileDialog.FileModeEnum.OpenFile;
        file_dialog.OkButtonText = "Import Playlist";
        file_dialog.Title = "Select Playlist";
    }
}
