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
}
