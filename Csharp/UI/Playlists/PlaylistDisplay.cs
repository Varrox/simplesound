using Godot;

public partial class PlaylistDisplay : Button
{
    [Export] public TextureRect Cover;
    [Export] public Label Name, Songs;
    [Export] public ContextMenu More;

    static readonly char dot = '\u00b7';

    int playlist_index;
    PlaylistsVisualizer visualizer;

    public override void _Ready()
    {
        More.Hide();
        ButtonDown += Set;
        MouseEntered += More.Show;
        MouseExited += OnExit;
        More.MouseEntered += More.Show;
        More.OnClose += More.Hide;
    }

    public void OnExit()
    {
        if (!More.menu_open) More.Hide();
    }

    public void Set()
    {
        Globals.main.playlistVisualizer.EmitSignal("OnSelectPlaylist", playlist_index, Cover.Texture);
        Globals.main.current_looked_at_playlist = playlist_index;
        SelfModulate = Globals.lower_highlight;
    }

    public void ClearSelected(int index, Texture2D img)
    {
        if (index != playlist_index) SelfModulate = new Color(1, 1, 1, 1);
    }

    public void Init(Playlist playlist, int index, bool current, Control menu)
    {
        Cover.Texture = ConvertToGodot.LoadImage(playlist.Cover, ref Globals.default_cover);

        Name.Text = playlist.Name;
        if (playlist.Type != Playlist.PlaylistType.Album)
        { 
            if (playlist.Songs == null)
                Songs.Text = "0 songs";
            else
                Songs.Text = (playlist.Songs.Count.ToString() + (playlist.Songs.Count != 1 ? " songs" : " song")) + (playlist.Artist != null ? $" {dot} {playlist.Artist}" : "");
        }
        else
        {
            Songs.Text = $"Album  {dot}  " + (playlist.Artist != null ? playlist.Artist : playlist.Songs.Count.ToString() + (playlist.Songs.Count != 1 ? " songs" : " song"));
        }

        More.menu = menu;

        playlist_index = index;

        if (current) Set();

        Globals.main.playlistVisualizer.OnSelectPlaylist += ClearSelected;
    }
}
