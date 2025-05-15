using Godot;

public partial class Globals : Node
{
    [Export] Texture2D _play_texture
    {
        set
        {
            play_texture = value;
        }
        get { return play_texture; }
    }

    public static Texture2D play_texture;

    [Export] Texture2D _pause_texture
    {
        set
        {
            pause_texture = value;
        }
        get { return pause_texture; }
    }

    public static Texture2D pause_texture;

    [Export] Texture2D _default_cover
    {
        set
        {
            default_cover = value;
        }
        get { return default_cover; }
    }

    public static Texture2D default_cover;

    [Export] Texture2D _default_cover_highres
    {
        set
        {
            default_cover_highres = value;
        }
        get { return default_cover_highres; }
    }

    public static Texture2D default_cover_highres;

    [Export] Main _main
    {
        set
        {
            main = value;
        }

        get
        {
            return main;
        }
    }

    public static Main main;

    [Export] Player _player
    {
        set
        {
            player = value;
        }

        get
        {
            return player;
        }
    }

    public static Player player;

    [Export] FileDialog _file_dialog
    {
        set
        {
            file_dialog = value;
        }
        get { return file_dialog; }
    }

    public static FileDialog file_dialog;

    [Export] Node _self
    {
        set
        {
            self = value;
        }
        get { return self; }
    }

    public static Node self;

    [Export]
    Color _highlight
    {
        set
        {
            highlight = value;
        }
        get
        {
            return highlight;
        }
    }

    public static Color highlight;

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

    public static void SetFileDialogCover()
    {
        file_dialog.Filters = new[] { "*.jpeg", "*.jpg", "*.png", "*.webp" };
        file_dialog.FileMode = FileDialog.FileModeEnum.OpenFile;
        file_dialog.OkButtonText = "Import Cover";
        file_dialog.Title = "Select Cover";
    }

    public static void SetFileDialogPlaylist()
    {
        file_dialog.Filters = new[] { "*.ssl"};
        file_dialog.FileMode = FileDialog.FileModeEnum.OpenFile;
        file_dialog.OkButtonText = "Import Playlist";
        file_dialog.Title = "Select Playlist";
    }
}
