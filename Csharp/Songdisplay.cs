using Godot;
using System;

public partial class Songdisplay : Control
{
	[Export] public Label Number, Name, Artist, Time;
	[Export] public Control Play;
    [Export] public TextureRect Cover, Playbutton;
    [Export] public Button Register;
	[Export] public ContextMenu More;
	[Export] public Texture2D PlayTexture, Pause;
	[Export] public Panel explicitLyrics;
	[Export] public Color highlight;

	int playlist, song;
	bool isPlaying;
	public override void _Ready()
	{
		Register.ButtonDown += SetSong;
		Register.MouseEntered += onEnter;
		Register.MouseExited += onExit;
		More.MouseEntered += onEnter;
		More.OnClose += () => More.Hide();
		(GetTree().CurrentScene as Main).OnLoadSong += SetHighlight;
	}

	public void SetHighlight()
	{
		Main Main = GetTree().CurrentScene as Main;

		if (Main.currentPlaylist == playlist && Main.currentSong == song) // highlight
		{
			Main.OnPlay += SetTextures;
			isPlaying = true;
			Register.SelfModulate = highlight;
		}
		else if (Main.currentPlaylist != playlist || Main.currentSong != song) // un-highlight
        {
			Playbutton.Texture = PlayTexture;
            Main.OnPlay -= SetTextures;
			isPlaying = false;
            Register.SelfModulate = new Color(1, 1, 1, 1);
        }

        SetTextures(isPlaying && Main.playing);
    }

	public void SetTextures(bool playing)
	{
        Playbutton.Texture = playing ? Pause : PlayTexture;
    }

	public void onEnter()
	{
        More.Show();
		Play.Show();
		Number.SelfModulate = new Color(1, 1, 1, 0);
    }

	public void onExit()
	{
		if(!More.menuOpen) More.Hide();
		Play.Hide();
        Number.SelfModulate = new Color(1, 1, 1, 1);
    }

    public void init(string name, string artist, string time, int playlist, int song, bool explicitLyrics, Texture2D cover, Control menu)
	{
		Number.Text = (song + 1).ToString();
		this.song = song;
		this.playlist = playlist;
		Name.Text = name;
		Artist.Text = artist;
		Time.Text = time;
		Cover.Texture = cover;
		More.menu = menu;
		this.explicitLyrics.Visible = explicitLyrics;
		SetHighlight();
    }

	public void SetSong()
	{
		Main Main = GetTree().CurrentScene as Main;

		if(Main.currentPlaylist != playlist)
		{
            Main.LoadPlaylist(playlist);
        }

		if(!isPlaying)
		{
            Main.currentSong = 0;
            Main.MoveSong(song, true);
        }
		else Main.Play();
	}
}
