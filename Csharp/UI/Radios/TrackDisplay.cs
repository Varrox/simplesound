using Godot;

public partial class TrackDisplay : Button {
    [Export] public RichTextLabel track_name_label;

    [Export] public Label track_number_label, track_artist_label, total_time_label;
    
    [Export] public TextureRect cover_texture_rect, play_texture_rect;

    [Export] public Control spacer, sound_visualizer;

    [Export] public TracksMore more;

    public int track_index, playlist_index;

    public bool highlighted;

    public TrackData track_data;

    public override void _Ready() {
        ButtonUp += SetTrack;
        MouseEntered += OnEnter;
        MouseExited += OnExit;

        more.MouseEntered += OnEnter;

        Main.instance.OnLoadTrack += SetHighlight;
    }

    public override void _ExitTree() {
        Main.instance.OnLoadTrack -= SetHighlight;
    }

    public override void _GuiInput(InputEvent @event) {
        if (@event is InputEventMouseButton) {
            if ((@event as InputEventMouseButton).ButtonIndex == MouseButton.Right) {
                more.PopupAtMouse();
            }
        }
    }

    public void SetHighlight() {
        Main.instance.OnPlayingChanged -= SetPlaying;

        highlighted = Main.playlist_index == playlist_index && Main.track_index == track_index;

        ApplicationManager.SetRadioSelected(this, highlighted);

        track_name_label.AddThemeColorOverride("font_color", ApplicationManager.GetThemeColor(highlighted ? "playing_font_color" : "normal_font_color"));

        if (highlighted) Main.instance.OnPlayingChanged += SetPlaying;

        SetPlaying();
    }

    public void SetPlaying() {
        sound_visualizer.Visible = highlighted && !IsHovered();

        track_number_label.AddThemeColorOverride("font_color", highlighted || IsHovered() ? Colors.Transparent : ApplicationManager.GetThemeColor("small_font_color"));
        play_texture_rect.Texture = ApplicationManager.GetThemeIcon(Disabled ? "no_play_icon" : (Main.playing && highlighted ? "pause_icon" : "play_icon"));
    }

    public void OnEnter() {
        more.Show();

        SetPlaying();

        play_texture_rect.Show();
    }

    public void OnExit() {
        if (!more.GetPopup().Visible) more.Hide();

        SetPlaying();

        play_texture_rect.Hide();
        play_texture_rect.Texture = null;
    }

    public void Init(in int track, in int playlist, in TrackData data, in Playlist.PlaylistType type, in Texture2D cover) {
        track_data = data;

        Disabled = data.corrupt;

        track_number_label.Text = (track + 1).ToString();

        track_index = track;
        playlist_index = playlist;

        track_name_label.Text = data.title + (data.explicit_lyrics ? " [img height=1.5em]Icons/SVG/explicit.svg[/img]" : "");
        track_artist_label.Text = data.artist;
        total_time_label.Text = data.time;

        bool album = type == Playlist.PlaylistType.Album;
        
        cover_texture_rect.Texture = cover;
        (cover_texture_rect.GetParent() as Control).Visible = !album;

        spacer.Visible = !album;

        more.GetPopup().SetItemDisabled(TracksMore.EDIT_METADATA, data.corrupt);
        more.share_popup_menu.SetItemDisabled(TracksMore.COPY_SHARE_LINK, data.corrupt);

        SetHighlight();
    }

    public void SetTrack() {
        if (Main.playlist_index != playlist_index) Main.instance.LoadPlaylist(playlist_index);
        if (!highlighted) Main.instance.SetTrack(track_index);
        else Main.instance.FlipPlayingState();
    }
}
