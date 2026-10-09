using Godot;

public partial class TrackDisplay : Button
{
    [Export] public Label track_number_label, track_name_label, track_artist_label, total_time_label;
    
    [Export] public TextureRect cover_texture_rect, play_texture_rect;

    [Export] public Control spacer, sound_visualizer;

    [Export] public TracksContextMenuOpener more;

    [Export] public Panel explicit_lyrics_indicator;

    public int track_index;
    public bool playing;

    public override void _Ready() {
        ButtonUp += SetTrack;
        MouseEntered += OnEnter;
        MouseExited += OnExit;

        more.MouseEntered += OnEnter;
        more.OnClose += more.Hide;

        Globals.main.OnLoadTrack += SetHighlight;
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
        Globals.main.OnPlayingChanged -= SetTextures;

        if (Globals.main.playlist_index == Globals.main.looked_at_playlist && Globals.main.track_index == track_index) { // highlight
            Globals.main.OnPlayingChanged += SetTextures;
            playing = true;
            ApplicationManager.SetRadioSelected(this, true);
            track_number_label.AddThemeColorOverride("font_color", Colors.Transparent);
            track_name_label.AddThemeColorOverride("font_color", ApplicationManager.theme.GetColor("playing_font_color", Constants.THEME_TYPE));
            if(!IsHovered()) sound_visualizer.Visible = true;
        }
        else if (Globals.main.playlist_index != Globals.main.looked_at_playlist || Globals.main.track_index != track_index) { // un-highlight
            play_texture_rect.Texture = ApplicationManager.theme.GetIcon(Disabled ? "no_play_icon" : "play_icon", Constants.THEME_TYPE);
            Globals.main.OnPlayingChanged -= SetTextures;
            playing = false;
            ApplicationManager.SetRadioSelected(this, false);
            track_number_label.AddThemeColorOverride("font_color", ApplicationManager.theme.GetColor("small_font_color", Constants.THEME_TYPE));
            track_name_label.AddThemeColorOverride("font_color", ApplicationManager.theme.GetColor("normal_font_color", Constants.THEME_TYPE));
            sound_visualizer.Visible = false;
        }

        SetTextures(playing && Globals.main.playing);
    }

    public void SetTextures(bool playing) {
        play_texture_rect.Texture = ApplicationManager.theme.GetIcon(Disabled ? "no_play_icon" : (playing ? "pause_icon" : "play_icon"), Constants.THEME_TYPE);
    }

    public void OnEnter() {
        more.Show();

        SetTextures(playing);

        play_texture_rect.Show();

        if (playing) sound_visualizer.Visible = false;
        track_number_label.AddThemeColorOverride("font_color", Colors.Transparent);
    }

    public void OnExit() {
        if (!more.menu_open) more.Hide();

        play_texture_rect.Hide();
        play_texture_rect.Texture = null;
        
        if (playing) sound_visualizer.Visible = true;
        else track_number_label.AddThemeColorOverride("font_color", ApplicationManager.theme.GetColor("small_font_color", Constants.THEME_TYPE));
    }

    public void Init(in int track, in TrackData data, in Playlist.PlaylistType type, in Texture2D cover) {
        Disabled = data.corrupt;

        track_number_label.Text = (track + 1).ToString();

        this.track_index = track;

        this.track_name_label.Text = data.title;
        this.track_artist_label.Text = data.artist;
        this.total_time_label.Text = data.time;

        bool album = type == Playlist.PlaylistType.Album;
        
        this.cover_texture_rect.Texture = cover;
        (this.cover_texture_rect.GetParent() as Control).Visible = !album;
        spacer.Visible = !album;
        this.explicit_lyrics_indicator.Visible = data.explicit_lyrics;

        SetHighlight();
    }

    public void SetTrack() {
        if (Globals.main.playlist_index != Globals.main.looked_at_playlist) Globals.main.LoadPlaylist(Globals.main.looked_at_playlist);
        if (!playing) Globals.main.SetTrack(track_index);
        else Globals.main.FlipPlayingState();
    }
}
