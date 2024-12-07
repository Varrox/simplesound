using Godot;
using System;

public partial class Songdisplay : Control
{
	[Export] public Label Number, Name, Artist, Time;
    [Export] public TextureRect Cover;
    [Export] public Button Register;

	int playlist, song;
	public override void _Ready()
	{
		Register.ButtonDown += SetSong;
	}

	public void init(string name, string artist, string time, int playlist, int song, ImageTexture cover)
	{
		Number.Text = (song + 1).ToString();
		this.song = song;
		this.playlist = playlist;
		Name.Text = name;
		Artist.Text = artist;
		Time.Text = time;
		Cover.Texture = cover;
	}

	public void SetSong()
	{
		Main Main = GetTree().CurrentScene as Main;
		Main.LoadPlaylist(playlist);
		Main.currentSong = 0;
		Main.MoveSong(song);
	}
}
