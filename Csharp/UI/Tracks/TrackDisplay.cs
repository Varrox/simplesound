using Godot;

public partial class TrackDisplay : Button
{
    [Export] public Label track_number_label, track_name_label, track_artist_label, total_time_label;
    
    [Export] public TextureRect cover_texture_rect, play_texture_rect;

    [Export] public Control spacer, sound_visualizer;

    [Export] public TracksContextMenuOpener more;

    [Export] public Panel explicit_lyrics_indicator;

    public int track_index, playlist_index;

    public bool playing;

    public override void _Ready() {
        ButtonUp += SetTrack;
        MouseEntered += OnEnter;
        MouseExited += OnExit;

        more.MouseEntered += OnEnter;
        more.OnClose += more.Hide;

        Globals.main.OnLoadTrack += SetHighlight;
    }

    public override void _ExitTree() {
        Globals.main.OnLoadTrack -= SetHighlight;
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
        Globals.main.OnPlayingChanged -= SetPlaying;

        bool highlight = Globals.main.playlist_index == playlist_index && Globals.main.track_index == track_index;

        playing = highlight;

        ApplicationManager.SetRadioSelected(this, highlight);

        track_name_label.AddThemeColorOverride("font_color", ApplicationManager.theme.GetColor(highlight ? "playing_font_color" : "normal_font_color", Constants.THEME_TYPE));

        if (highlight) Globals.main.OnPlayingChanged += SetPlaying;

        SetPlaying(playing && Globals.main.playing);
    }

    public void SetPlaying(bool playing) {
        sound_visualizer.Visible = playing && !IsHovered();

        track_number_label.AddThemeColorOverride("font_color", playing || IsHovered() ? Colors.Transparent : ApplicationManager.theme.GetColor("small_font_color", Constants.THEME_TYPE));
        play_texture_rect.Texture = ApplicationManager.theme.GetIcon(Disabled ? "no_play_icon" : (playing ? "pause_icon" : "play_icon"), Constants.THEME_TYPE);
    }

    public void OnEnter() {
        more.Show();

        SetPlaying(playing);

        play_texture_rect.Show();
    }

    public void OnExit() {
        if (!more.menu_open) more.Hide();

        SetPlaying(playing);

        play_texture_rect.Hide();
        play_texture_rect.Texture = null;
    }

    public void Init(in int track, in int playlist, in TrackData data, in Playlist.PlaylistType type, in Texture2D cover) {
        Disabled = data.corrupt;

        track_number_label.Text = (track + 1).ToString();

        track_index = track;
        playlist_index = playlist;

        track_name_label.Text = data.title;
        track_artist_label.Text = data.artist;
        total_time_label.Text = data.time;

        bool album = type == Playlist.PlaylistType.Album;
        
        cover_texture_rect.Texture = cover;
        (cover_texture_rect.GetParent() as Control).Visible = !album;

        spacer.Visible = !album;

        explicit_lyrics_indicator.Visible = data.explicit_lyrics;

        SetHighlight();
    }

    public void SetTrack() {
        if (Globals.main.playlist_index != playlist_index) Globals.main.LoadPlaylist(playlist_index);
        if (!playing) Globals.main.SetTrack(track_index);
        else Globals.main.FlipPlayingState();
    }
}
