using Godot;
using System;

public partial class Songdisplay : Button
{
	[Export] public Label Number, Name, Artist, Time;
    [Export] public TextureRect Cover, Playbutton;
	[Export] public Control Spacer;
	[Export] public ContextMenu More;
	[Export] public Panel explicitLyrics;

	int playlist, song;
	public bool isPlaying;
	public override void _Ready()
	{
		ButtonUp += SetSong;
		MouseEntered += OnEnter;
		MouseExited += OnExit;
		More.MouseEntered += OnEnter;
		More.OnClose += More.Hide;
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
			SelfModulate = Globals.highlight;
		}
		else if (Globals.main.currentPlaylist != playlist || Globals.main.currentSong != song) // un-highlight
        {
			Playbutton.Texture = Globals.play_texture;
            Globals.main.OnPlay -= SetTextures;
			isPlaying = false;
            SelfModulate = new Color(1, 1, 1, 1);
        }

        SetTextures(isPlaying && Globals.main.playing);
    }

	public void SetTextures(bool playing)
	{
		Playbutton.Texture = playing ? Globals.pause_texture : Globals.play_texture;
    }

	public void OnEnter()
	{
        More.Show();
		SetTextures(isPlaying);
        Playbutton.Show();
        Number.SelfModulate = new Color(1, 1, 1, 0);
    }

	public void OnExit()
	{
		if(!More.menuOpen) More.Hide();
        Playbutton.Hide();
		Playbutton.Texture = null;
        Number.SelfModulate = new Color(1, 1, 1, 1);
    }

    public void Init(string name, string artist, string time, int playlist, int song, bool explicitLyrics, Playlist.PlaylistType type, Texture2D cover, Control menu)
	{
		Number.SetThreadSafe("text", (song + 1).ToString());

		this.song = song;
		this.playlist = playlist;

		Name.SetThreadSafe("text", name);
		Artist.SetThreadSafe("text", artist);
		Time.SetThreadSafe("text", time);

        Cover.SetThreadSafe("texture", cover);

        bool album = type == Playlist.PlaylistType.Album;

        (Cover.GetParent() as Control).SetThreadSafe("visible", !album);
        Spacer.SetThreadSafe("visible", !album);
        this.explicitLyrics.SetThreadSafe("visible", explicitLyrics);

        More.menu = menu;

		CallThreadSafe("SetHighlight");
    }

	public void SetSong()
	{
		if(Globals.main.currentPlaylist != playlist) Globals.main.LoadPlaylist(playlist);
		if(!isPlaying) Globals.main.SetSong(song);
		else Globals.main.Play();
	}
}
