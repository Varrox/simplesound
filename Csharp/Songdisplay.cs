using Godot;
using System;

public partial class Songdisplay : Control
{
	[Export] public Label Number, Name, Artist, Time;
	[Export] public Control Play;
    [Export] public TextureRect Cover;
    [Export] public Button Register;
	[Export] public ContextMenu More;

	int playlist, song;
	public override void _Ready()
	{
		Register.ButtonDown += SetSong;
		Register.MouseEntered += onEnter;
		Register.MouseExited += onExit;
		More.MouseEntered += onEnter;
	}

	public void onEnter()
	{
        More.Show();
		Play.Show();
		Number.SelfModulate = new Color(1, 1, 1, 0);
    }

	public void onExit()
	{
        More.Hide();
		Play.Hide();
        Number.SelfModulate = new Color(1, 1, 1, 1);
    }

    public void init(string name, string artist, string time, int playlist, int song, ImageTexture cover, Control menu)
	{
		Number.Text = (song + 1).ToString();
		this.song = song;
		this.playlist = playlist;
		Name.Text = name;
		Artist.Text = artist;
		Time.Text = time;
		Cover.Texture = cover;
		More.menu = menu;
	}

	public void SetSong()
	{
		Main Main = GetTree().CurrentScene as Main;
		Main.LoadPlaylist(playlist);
		Main.currentSong = 0;
		Main.MoveSong(song);
	}
}
