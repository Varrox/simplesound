using Godot;

public partial class PlaylistDisplay : Node
{
    [Export] public TextureRect Cover;
    [Export] public Label Name, Songs;
    [Export] public Button Register;
    [Export] public ContextMenu More;
    [Export] public Color SelectedColor;

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


    public void init(string playlistname, string Coverpath, int songcount, int index, PlaylistsVisualizer visualizer, bool current, Control menu)
    {
        var img = new Image();
        bool nuhuh = Coverpath == null;
        if (Coverpath != null)
        {
            if (img.Load(Coverpath) == Error.Ok) Cover.Texture = ImageTexture.CreateFromImage(img);
            else nuhuh = true;
        }
        if(nuhuh) Cover.Texture = ResourceLoader.Load<Texture2D>("res://Icons/DefaultCover.png");

        Name.Text = playlistname;
        Songs.Text = songcount.ToString() + (songcount > 1 ? " songs" : " song");
        More.menu = menu;

        PlaylistIndex = index;

        this.visualizer = visualizer;

        if (current) Set();

        visualizer.OnSelectPlaylist += clearSelected;
    }
}
