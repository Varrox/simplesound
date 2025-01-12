using Godot;

public partial class PlaylistDisplay : Node
{
    [Export] public TextureRect Cover;
    [Export] public Label Name, Songs;
    [Export] public Button Register;
    [Export] public ContextMenu More;
    [Export] public Color SelectedColor;

    public const char dot = '\u00b7';

    int PlaylistIndex;
    PlaylistsVisualizer visualizer;

    public override void _Ready()
    {
        Register.ButtonDown += Set;
        Register.MouseEntered += () => More.Show();
        Register.MouseExited += () => More.Hide();
        More.MouseEntered += () => More.Show();
    }

    public void Set()
    {
        visualizer.EmitSignal("OnSelectPlaylist", PlaylistIndex, Cover.Texture);
        Register.SelfModulate = SelectedColor;
    }

    public void clearSelected(int index, Texture2D img)
    {
        if (index != PlaylistIndex) Register.SelfModulate = new Color(1, 1, 1, 1);
    }


    public void init(Playlist playlist, int index, PlaylistsVisualizer visualizer, bool current, Control menu)
    {
        bool nuhuh = playlist.Coverpath == null;

        if (playlist.Coverpath != null)
        {
            var img = new Image();
            if (img.Load(playlist.Coverpath) == Error.Ok) Cover.Texture = ImageTexture.CreateFromImage(img);
            else nuhuh = true;
        }
        if(nuhuh) Cover.Texture = visualizer.defaultCover;

        Name.Text = playlist.Name;
        if (playlist.type != Playlist.PlaylistType.Album)
        { 
            Songs.Text = (playlist.songs.Count.ToString() + (playlist.songs.Count != 1 ? " songs" : " song")) + (playlist.artist != null ? $" {dot} {playlist.artist}" : "");
        }
        else
        {
            Songs.Text = $"Album  {dot}  " + (playlist.artist != null ? playlist.artist : playlist.songs.Count.ToString() + (playlist.songs.Count != 1 ? " songs" : " song"));
        }

        More.menu = menu;

        PlaylistIndex = index;

        this.visualizer = visualizer;

        if (current) Set();

        visualizer.OnSelectPlaylist += clearSelected;
    }
}
