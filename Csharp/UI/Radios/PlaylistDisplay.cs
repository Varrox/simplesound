using Godot;

public partial class PlaylistDisplay : Button {
    [Export] public TextureRect cover_texture_rect;
    [Export] public Label playlist_name_label, track_count_label;
    [Export] public PlaylistsMore more;

    public int playlist_index;

    public override void _Ready() {
        ButtonUp += Set;

        MouseEntered += more.Show;
        MouseExited += OnExit;

        Main.instance.OnLoadTrack += SetTextHighlight;
    }

    public void SetTextHighlight() {
        bool highlight = Main.playlist_index == playlist_index;
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
        Main.instance.playlist_visualizer.EmitSignal("OnSelectPlaylist", playlist_index);
        Main.looked_at_playlist = playlist_index;

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

        if (index == Main.looked_at_playlist) Set();

        Main.instance.playlist_visualizer.OnSelectPlaylist += ClearSelected;
    }
}
