using Godot;

public partial class PlaylistDisplay : Button {
    [Export] public TextureRect cover_texture_rect;
    [Export] public Label playlist_name_label, track_count_label;
    [Export] public PlaylistsMore more;

    int playlist_index;

    public override void _Ready() {
        ButtonUp += Set;

        MouseEntered += more.Show;
        MouseExited += OnExit;

        Globals.main.OnLoadTrack += SetTextHighlight;
    }

    public void SetTextHighlight() {
        bool highlight = Globals.main.playlist_index == playlist_index;
        playlist_name_label.AddThemeColorOverride("font_color", ApplicationManager.GetThemeColor(highlight ? "playing_font_color" : "normal_font_color"));
    }

    public override void _GuiInput(InputEvent @event) {
        if (@event is InputEventMouseButton) {
            if ((@event as InputEventMouseButton).ButtonIndex == MouseButton.Right) {
                more.PopupAtMouse();
            }
        }
    }

    public void OnExit() {
        if (!more.GetPopup().Visible) more.Hide();
    }

    public void Set() {
        Globals.main.playlist_visualizer.EmitSignal("OnSelectPlaylist", playlist_index);
        Globals.main.looked_at_playlist = playlist_index;

        ApplicationManager.SetRadioSelected(this, true);
    }

    public void ClearSelected(int index) {
        if (index != playlist_index) ApplicationManager.SetRadioSelected(this, false);
    }

    public void Init(Playlist playlist, int index) {
        cover_texture_rect.Texture = playlist.cover_texture ?? ApplicationManager.GetThemeIcon("default_cover_icon");
        playlist_name_label.Text = playlist.name;

        int track_count = playlist.tracks == null ? 0 : playlist.tracks.Count;

        string amount = $"{track_count}{(track_count != 1 ? " tracks" : " track")}";

        if (playlist.type != Playlist.PlaylistType.Album) {
            if (playlist.artist == "")
                track_count_label.Text = amount;
            else
                track_count_label.Text = $"{amount} {Constants.DOT} {playlist.artist}";
        }
        else {
            track_count_label.Text = $"Album  {Constants.DOT}  {playlist.artist ?? amount}";
        }

        playlist_index = index;

        TooltipText = $"{playlist.name} - {amount}";

        if (index == Globals.main.looked_at_playlist) Set();

        Globals.main.playlist_visualizer.OnSelectPlaylist += ClearSelected;
    }
}
