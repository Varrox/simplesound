using Godot;
using static System.Runtime.InteropServices.JavaScript.JSType;

public partial class PlaylistDisplay : Button
{
    [Export] public TextureRect Cover;
    [Export] public Label Name, Songs;
    [Export] public ContextMenu More;

    static readonly char dot = '\u00b7';

    int PlaylistIndex;
    PlaylistsVisualizer visualizer;

    public override void _Ready()
    {
        More.Hide();
        ButtonDown += Set;
        MouseEntered += () => More.Show();
        MouseExited += onExit;
        More.MouseEntered += () => More.Show();
        More.OnClose += () => More.Hide();
    }

    public void onExit()
    {
        if (!More.menuOpen) More.Hide();
    }

    public void Set()
    {
        Globals.main.playlistvisualizer.EmitSignal("OnSelectPlaylist", PlaylistIndex, Cover.Texture);
        Globals.main.currentLookingAtPlaylist = PlaylistIndex;
        SelfModulate = Globals.lower_highlight;
    }

    public void clearSelected(int index, Texture2D img)
    {
        if (index != PlaylistIndex) SelfModulate = new Color(1, 1, 1, 1);
    }

    public void init(Playlist playlist, int index, bool current, Control menu)
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

        PlaylistIndex = index;

        if (current) Set();

        Globals.main.playlistvisualizer.OnSelectPlaylist += clearSelected;
    }
}
