using Godot;

public partial class PlaylistDisplay : Button
{
    [Export] public TextureRect Cover;
    [Export] public Label Name, Songs;
    [Export] public ContextMenu More;

    const char dot = '\u00b7';

    int playlist_index;

    public override void _Ready()
    {
        More.Hide();
        ButtonDown += Set;

        MouseEntered += More.Show;
        MouseExited += OnExit;

        More.MouseEntered += More.Show;
        More.OnClose += More.Hide;
        More.menu = Globals.playlist_menu;
    }

    public void OnExit()
    {
        if (!More.menu_open) More.Hide();
    }

    public void Set()
    {
        Globals.main.playlist_visualizer.OnSelectPlaylist?.Invoke(playlist_index, Cover.Texture);
        Globals.main.looked_at_playlist = playlist_index;
        SelfModulate = Globals.lower_highlight;
    }

    public void ClearSelected(int index, Texture2D img)
    {
        if (index != playlist_index) SelfModulate = new Color(1, 1, 1, 1);
    }

    public void Init(Playlist playlist, int index)
    {
        Cover.Texture = ConvertToGodot.LoadImage(playlist.cover) ?? Globals.default_cover;
        Name.Text = playlist.name;

        if (playlist.type != Playlist.PlaylistType.Album)
        {
            if (playlist.songs == null)
                Songs.Text = "0 songs";
            else
                Songs.Text = $"{playlist.songs.Count}{(playlist.songs.Count != 1 ? " songs" : " song")}{(playlist.artist != null ? $" {dot} {playlist.artist}" : "")}";
        }
        else
        {
            Songs.Text = $"Album  {dot}  {playlist.artist ?? (playlist.songs.Count.ToString() + (playlist.songs.Count != 1 ? " songs" : " song"))}";
        }

        playlist_index = index;

        if (index == Globals.main.looked_at_playlist) Set();

        Globals.main.playlist_visualizer.OnSelectPlaylist += ClearSelected;
    }
}
