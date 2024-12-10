using Godot;
using System;

public partial class PlaylistDisplay : Node
{
    [Export] public TextureRect Cover;
    [Export] public Label Name, Songs;
    [Export] public Button Register, More;
    [Export] public Color SelectedColor;

    int PlaylistIndex;
    PlaylistsVisualizer visualizer;

    public override void _Ready()
    {
        Register.ButtonDown += Set;
        Register.MouseEntered += onEnter;
        Register.MouseExited += onExit;
        More.MouseEntered += onEnter;
    }


    public void onEnter()
    {
        More.Show();
    }

    public void onExit()
    {
        More.Hide();
    }

    public void Set() // reprogram to load song visualizer
    {
        visualizer.EmitSignal("OnSelectPlaylist", PlaylistIndex, Cover.Texture);
        Register.SelfModulate = SelectedColor;
    }

    public void clearSelected(int index, Texture2D img)
    {
        if (index != PlaylistIndex)
        {
            Register.SelfModulate = new Color(1, 1, 1, 1);
        }
    }


    public void init(string playlistname, string Coverpath, int songcount, int index, PlaylistsVisualizer visualizer)
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
        Songs.Text = songcount.ToString() + " songs";

        PlaylistIndex = index;

        this.visualizer = visualizer;

        visualizer.OnSelectPlaylist += clearSelected;
    }
}
