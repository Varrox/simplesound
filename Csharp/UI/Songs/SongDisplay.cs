using Godot;

public partial class SongDisplay : Button
{
    [Export] public Label number, name, artist, time;
    [Export] public TextureRect cover, play_button;
    [Export] public Control spacer;
    [Export] public SongsMore more;
    [Export] public Panel explicit_lyrics;

    public int song;
    public bool playing;
    public bool is_visible = false;
    public override void _Ready()
    {
        ButtonUp += SetSong;
        MouseEntered += OnEnter;
        MouseExited += OnExit;
        more.MouseEntered += OnEnter;
        more.OnClose += more.Hide;
        Globals.main.OnLoadSong += SetHighlight;
    }

    public override void _Input(InputEvent @event)
    {
        if (is_visible)
        {
            if (@event is InputEventMouseButton)
            {
                if ((@event as InputEventMouseButton).ButtonIndex == MouseButton.Right)
                {
                    if (GetGlobalRect().HasPoint(GetGlobalMousePosition()))
                    {
                        more.OpenMenu();
                        more.menu.GlobalPosition = GetGlobalMousePosition();
                    }
                }
            }
        }
    }

    public void SetHighlight()
    {
        playing = false;
        Globals.main.OnPlay -= SetTextures;

        if (Globals.main.playlist_index == Globals.main.looked_at_playlist && Globals.main.song_index == song) // highlight
        {
            Globals.main.OnPlay += SetTextures;
            playing = true;
            SelfModulate = Globals.highlight;
        }
        else if (Globals.main.playlist_index != Globals.main.looked_at_playlist || Globals.main.song_index != song) // un-highlight
        {
            play_button.Texture = Globals.play_texture;
            Globals.main.OnPlay -= SetTextures;
            playing = false;
            SelfModulate = new Color(1, 1, 1, 1);
        }

        SetTextures(playing && Globals.main.playing);
    }

    public void SetTextures(bool playing)
    {
        play_button.Texture = playing ? Globals.pause_texture : Globals.play_texture;
    }

    public void OnEnter()
    {
        more.Show();
        SetTextures(playing);
        play_button.Show();
        number.SelfModulate = new Color(1, 1, 1, 0);
    }

    public void OnExit()
    {
        if (!more.menu_open) more.Hide();
        play_button.Hide();
        play_button.Texture = null;
        number.SelfModulate = new Color(1, 1, 1, 1);
    }

    public void Init(string name, string artist, string time, int song, bool explicit_lyrics, Playlist.PlaylistType type, Texture2D cover)
    {
        number.SetThreadSafe("text", (song + 1).ToString());

        this.song = song;


        this.name.SetThreadSafe("text", name);
        this.artist.SetThreadSafe("text", artist);
        this.time.SetThreadSafe("text", time);

        this.cover.SetThreadSafe("texture", cover);

        SetThreadSafe("process_mode", (int)(cover == null ? ProcessModeEnum.Disabled : ProcessModeEnum.Inherit));

        bool album = type == Playlist.PlaylistType.Album;

        (this.cover.GetParent() as Control).SetThreadSafe("visible", !album);
        spacer.SetThreadSafe("visible", !album);
        this.explicit_lyrics.SetThreadSafe("visible", explicit_lyrics);

        CallThreadSafe("SetHighlight");
    }

    public void SetSong()
    {
        if (Globals.main.playlist_index != Globals.main.looked_at_playlist) Globals.main.LoadPlaylist(Globals.main.looked_at_playlist);
        if (!playing) Globals.main.SetSong(song);
        else Globals.main.Play();
    }
}
