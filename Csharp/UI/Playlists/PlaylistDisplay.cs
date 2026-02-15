using Godot;

public partial class PlaylistDisplay : Button
{
    [Export] public TextureRect cover;
    [Export] public Label playlist_name, songs;
    [Export] public ContextMenuOpener more;

    int playlist_index;

    public override void _Ready()
    {
        ButtonUp += Set;

        MouseEntered += more.Show;
        MouseExited += OnExit;
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
        Globals.main.playlist_visualizer.OnSelectPlaylist?.Invoke(playlist_index, cover.Texture);
        Globals.main.looked_at_playlist = playlist_index;
        SelfModulate = Globals.lower_highlight;
    }

    public void ClearSelected(int index, Texture2D img)
    {
        if (index != playlist_index) SelfModulate = new Color(1, 1, 1, 1);
    }

    public void Init(Playlist playlist, int index)
    {
        cover.Texture = ConvertToGodot.LoadImage(playlist.cover) ?? Globals.default_cover;
        playlist_name.Text = playlist.name;

        string amount = $"{playlist.songs.Count}{(playlist.songs.Count != 1 ? " songs" : " song")}";

        if (playlist.type != Playlist.PlaylistType.Album)
        {
            if (playlist.songs == null)
                songs.Text = "0 songs";
            else
                songs.Text = $"{amount}{(playlist.artist != null ? $" {Constants.dot} {playlist.artist}" : "")}";
        }
        else
        {
            songs.Text = $"Album  {Constants.dot}  {playlist.artist ?? (playlist.songs.Count.ToString() + (playlist.songs.Count != 1 ? " songs" : " song"))}";
        }

        playlist_index = index;

        TooltipText = $"{playlist.name} - {amount}";

        if (index == Globals.main.looked_at_playlist) Set();

        Globals.main.playlist_visualizer.OnSelectPlaylist += ClearSelected;
    }
}
