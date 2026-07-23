using Godot;

public partial class SongDisplay : Button
{
    [Export] public Label number, song_name, artist, time;
    [Export] public TextureRect cover, play_button;
    [Export] public Control spacer, sound_visualizer;
    [Export] public SongsMore more;
    [Export] public Panel explicit_lyrics;

    public int song;
    public bool playing;

    public override void _Ready() {
        ButtonUp += SetSong;
        MouseEntered += OnEnter;
        MouseExited += OnExit;
        more.MouseEntered += OnEnter;
        more.OnClose += more.Hide;
        Globals.main.OnLoadSong += SetHighlight;
    }

    public override void _Input(InputEvent @event) {
        if (@event is InputEventMouseButton) {
            if ((@event as InputEventMouseButton).ButtonIndex == MouseButton.Right) {
                if (GetGlobalRect().HasPoint(GetGlobalMousePosition())) {
                    more.OpenMenu();
                    more.menu.GlobalPosition = GetGlobalMousePosition();
                }
            }
        }
    }

    public void SetHighlight() {
        playing = false;
        Globals.main.OnPlay -= SetTextures;

        if (Globals.main.playlist_index == Globals.main.looked_at_playlist && Globals.main.song_index == song) { // highlight
            Globals.main.OnPlay += SetTextures;
            playing = true;
            SelfModulate = Globals.highlight;
            number.SelfModulate = new Color(1, 1, 1, 0);
            song_name.AddThemeColorOverride("font_color", Globals.playing_text_highlight);
            if(!IsHovered()) sound_visualizer.Visible = true;
        }
        else if (Globals.main.playlist_index != Globals.main.looked_at_playlist || Globals.main.song_index != song) { // un-highlight
            play_button.Texture = Disabled ? Globals.no_play_texture : Globals.play_texture;
            Globals.main.OnPlay -= SetTextures;
            playing = false;
            number.SelfModulate = new Color(1, 1, 1, 1);
            SelfModulate = Colors.White;
            song_name.AddThemeColorOverride("font_color", Colors.White);
            sound_visualizer.Visible = false;
        }

        SetTextures(playing && Globals.main.playing);
    }

    public void SetTextures(bool playing) {
        play_button.Texture = Disabled ? Globals.no_play_texture : (playing ? Globals.pause_texture : Globals.play_texture);
    }

    public void OnEnter() {
        more.Show();

        SetTextures(playing);

        play_button.Show();

        if (playing) sound_visualizer.Visible = false;
        number.SelfModulate = new Color(1, 1, 1, 0);
    }

    public void OnExit() {
        if (!more.menu_open) more.Hide();

        play_button.Hide();
        play_button.Texture = null;
        
        if (playing) sound_visualizer.Visible = true;
        else number.SelfModulate = new Color(1, 1, 1, 1);
    }

    public void Init(in int song, in SongData data, in Playlist.PlaylistType type, in Texture2D cover) {
        Disabled = data.corrupt;

        number.Text = (song + 1).ToString();

        this.song = song;

        this.song_name.Text = data.title;
        this.artist.Text = data.artist;
        this.time.Text = data.time;

        bool album = type == Playlist.PlaylistType.Album;
        
        this.cover.Texture = cover;
        (this.cover.GetParent() as Control).Visible = !album;
        spacer.Visible = !album;
        this.explicit_lyrics.Visible = data.explicit_lyrics;

        SetHighlight();
    }

    public void SetSong() {
        if (Globals.main.playlist_index != Globals.main.looked_at_playlist) Globals.main.LoadPlaylist(Globals.main.looked_at_playlist);
        if (!playing) Globals.main.SetSong(song);
        else Globals.main.Play();
    }
}
