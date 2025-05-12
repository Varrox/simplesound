using Godot;
using System;

public partial class Songdisplay : Control
{
	[Export] public Label Number, Name, Artist, Time;
    [Export] public TextureRect Cover, Playbutton;
	[Export] public Control Spacer;
    [Export] public Button Register;
	[Export] public ContextMenu More;
	[Export] public Panel explicitLyrics;

	int playlist, song;
	public bool isPlaying;
	public override void _Ready()
	{
		Register.ButtonUp += SetSong;
		Register.MouseEntered += onEnter;
		Register.MouseExited += onExit;
		More.MouseEntered += onEnter;
		More.OnClose += () => More.Hide();
		Globals.main.OnLoadSong += SetHighlight;
	}

	public void SetHighlight()
	{
		isPlaying = false;
        Globals.main.OnPlay -= SetTextures;

        if (Globals.main.currentPlaylist == playlist && Globals.main.currentSong == song) // highlight
		{
			Globals.main.OnPlay += SetTextures;
			isPlaying = true;
			Register.SelfModulate = Globals.highlight;
		}
		else if (Globals.main.currentPlaylist != playlist || Globals.main.currentSong != song) // un-highlight
        {
			Playbutton.Texture = Globals.play_texture;
            Globals.main.OnPlay -= SetTextures;
			isPlaying = false;
            Register.SelfModulate = new Color(1, 1, 1, 1);
        }

        SetTextures(isPlaying && Globals.main.playing);
    }

	public void SetTextures(bool playing)
	{
		Playbutton.Texture = playing ? Globals.pause_texture : Globals.play_texture;
    }

	public void onEnter()
	{
        More.Show();
        Playbutton.Show();
		Number.SelfModulate = new Color(1, 1, 1, 0);
    }

	public void onExit()
	{
		if(!More.menuOpen) More.Hide();
        Playbutton.Hide();
        Number.SelfModulate = new Color(1, 1, 1, 1);
    }

    public void init(string name, string artist, string time, int playlist, int song, bool explicitLyrics, SongsVisualizer visualizer, Texture2D cover, Control menu)
	{
		Number.Text = (song + 1).ToString();
		this.song = song;
		this.playlist = playlist;
		Name.Text = name;
		Artist.Text = artist;
		Time.Text = time;

		if (visualizer.Playlist.Type == Playlist.PlaylistType.Album)
		{
            (Cover.GetParent() as Control).Visible = false;
			Spacer.Visible = false;
		}
		else
		{
            Cover.Texture = cover;
            (Cover.GetParent() as Control).Visible = true;
            Spacer.Visible = true;
        }

        More.menu = menu;
		this.explicitLyrics.Visible = explicitLyrics;

		SetHighlight();
    }

	public void SetSong()
	{
		if(Globals.main.currentPlaylist != playlist)
		{
            Globals.main.LoadPlaylist(playlist);
        }

		if(!isPlaying)
		{
            Globals.main.currentSong = 0;
            Globals.main.MoveSong(song, true);
        }
		else Globals.main.Play();
	}
}
