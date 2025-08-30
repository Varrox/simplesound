using Godot;
using System;

public partial class SongDisplay : Button
{
	[Export] public Label number, name, artist, time;
    [Export] public TextureRect cover, play_button;
	[Export] public Control spacer;
	[Export] public ContextMenu more;
	[Export] public Panel explicit_lyrics;

	int playlist, song;
	public bool playing;
	public override void _Ready()
	{
		ButtonUp += SetSong;
		MouseEntered += OnEnter;
		MouseExited += OnExit;
		more.MouseEntered += OnEnter;
		more.OnClose += more.Hide;
		Globals.main.OnLoadSong += SetHighlight;
	}

	public void SetHighlight()
	{
		playing = false;
        Globals.main.OnPlay -= SetTextures;

        if (Globals.main.currentPlaylist == playlist && Globals.main.currentSong == song) // highlight
		{
			Globals.main.OnPlay += SetTextures;
			playing = true;
			SelfModulate = Globals.highlight;
		}
		else if (Globals.main.currentPlaylist != playlist || Globals.main.currentSong != song) // un-highlight
        {
			play_button.Texture = Globals.play_texture;
            Globals.main.OnPlay -= SetTextures;
			playing = false;
            SelfModulate = new Color(1, 1, 1, 1);
        }

        SetTextures(playing && Globals.main.playing);
    }

	public void SetTextures(bool playing)
	{
		play_button.Texture = playing ? Globals.pause_texture : Globals.play_texture;
    }

	public void OnEnter()
	{
        more.Show();
		SetTextures(playing);
        play_button.Show();
        number.SelfModulate = new Color(1, 1, 1, 0);
    }

	public void OnExit()
	{
		if(!more.menuOpen) more.Hide();
        play_button.Hide();
		play_button.Texture = null;
        number.SelfModulate = new Color(1, 1, 1, 1);
    }

    public void Init(string name, string artist, string time, int playlist, int song, bool explicit_lyrics, Playlist.PlaylistType type, Texture2D cover, Control menu)
	{
		number.SetThreadSafe("text", (song + 1).ToString());

		this.song = song;
		this.playlist = playlist;

		this.name.SetThreadSafe("text", name);
		this.artist.SetThreadSafe("text", artist);
		this.time.SetThreadSafe("text", time);

        this.cover.SetThreadSafe("texture", cover);

        SetThreadSafe("process_mode", (int)(cover == null ? ProcessModeEnum.Disabled : ProcessModeEnum.Inherit));

        bool album = type == Playlist.PlaylistType.Album;

        (this.cover.GetParent() as Control).SetThreadSafe("visible", !album);
        spacer.SetThreadSafe("visible", !album);
        this.explicit_lyrics.SetThreadSafe("visible", explicit_lyrics);

        more.menu = menu;

		CallThreadSafe("SetHighlight");
    }

	public void SetSong()
	{
		if(Globals.main.currentPlaylist != playlist) Globals.main.LoadPlaylist(playlist);
		if(!playing) Globals.main.SetSong(song);
		else Globals.main.Play();
	}
}
