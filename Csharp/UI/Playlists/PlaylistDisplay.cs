using Godot;

public partial class PlaylistDisplay : Button
{
    [Export] public TextureRect cover_texture_rect;
    [Export] public Label playlist_name_label, track_count_label;
    [Export] public ContextMenuOpener more;

    int playlist_index;

    public override void _Ready()
    {
        ButtonUp += Set;

        MouseEntered += more.Show;
        MouseExited += OnExit;

        Globals.main.OnLoadTrack += SetTextHighlight;
    }

    public void SetTextHighlight()
    {
        bool highlight = Globals.main.playlist_index == playlist_index;
        if(highlight)
            playlist_name_label.AddThemeColorOverride("font_color", Globals.playing_font_color);
        else
            playlist_name_label.AddThemeColorOverride("font_color", Colors.White);
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

    public void OnExit()
    {
        if (!more.menu_open) more.Hide();
    }

    public void Set()
    {
        Globals.main.playlist_visualizer.OnSelectPlaylist?.Invoke(playlist_index, cover_texture_rect.Texture);
        Globals.main.looked_at_playlist = playlist_index;
        SelfModulate = Globals.lower_highlight;
    }

    public void ClearSelected(int index, Texture2D img)
    {
        if (index != playlist_index) SelfModulate = Colors.White;
    }

    public void Init(Playlist playlist, int index)
    {
        cover_texture_rect.Texture = ConvertToGodot.LoadImageFromFile(playlist.cover) ?? Globals.default_cover;
        playlist_name_label.Text = playlist.name;

        string amount = $"{playlist.songs.Count}{(playlist.songs.Count != 1 ? " tracks" : " track")}";

        if (playlist.type != Playlist.PlaylistType.Album)
        {
            if (playlist.songs == null)
                track_count_label.Text = "0 tracks";
            else
                track_count_label.Text = $"{amount}{(playlist.artist != null ? $" {Constants.DOT} {playlist.artist}" : "")}";
        }
        else
        {
            track_count_label.Text = $"Album  {Constants.DOT}  {playlist.artist ?? (playlist.songs.Count.ToString() + (playlist.songs.Count != 1 ? " tracks" : " track"))}";
        }

        playlist_index = index;

        TooltipText = $"{playlist.name} - {amount}";

        if (index == Globals.main.looked_at_playlist) Set();

        Globals.main.playlist_visualizer.OnSelectPlaylist += ClearSelected;
    }
}
