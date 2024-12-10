using Godot;
using System;

public partial class PlaylistDisplay : Node
{
    [Export] public TextureRect Cover;
    [Export] public Label Name, Songs;
    [Export] public Button Register, More;
    [Export] public Color SelectedColor;

    int PlaylistIndex;

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

    public void Set()
    {
        var MainController = GetTree().CurrentScene as Main;

        if (MainController != null && MainController.currentPlaylist != PlaylistIndex)
        {
            MainController.currentSong = 0;
            MainController.LoadPlaylist(PlaylistIndex);
            MainController.time = 0;

            MainController.playing = true;

            MainController.InitSong();
            MainController.playing = false;

            MainController.Play();
            Register.SelfModulate = SelectedColor;
        }
    }

    public void clearSelected(int index)
    {
        if (index != PlaylistIndex)
        {
            Register.SelfModulate = new Color(1, 1, 1, 1);
        }
    }


    public void init(string playlistname, string Coverpath, int songcount, int index)
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

        (GetTree().CurrentScene as Main).OnLoadPlaylist += clearSelected;
    }
}
