using Godot;
using System;
using System.Linq;
using System.Collections.Generic;

public partial class Main : Control
{
	[Export] public AudioStreamPlayer player;
	[Export] public Slider volumeSlider;
	[Export] public PlaylistsVisualizer playlistvisualizer;
	[Export] public SongsVisualizer songsVisualizer;

	public bool loop, playing, shuffle;

	public int currentPlaylist, currentSong;
	public Playlist playlist;
	public string[] playlists;

	public List<int> shuffledlist = new List<int>();
	
	public float time, volume;

	[Signal] public delegate void OnLoadSongEventHandler();
	[Signal] public delegate void OnInitDoneEventHandler();
	[Signal] public delegate void OnPlayEventHandler(bool playing);

	public string song
	{
		get
		{
			return playlist.songs[songindex];
        }
	}

    public int songindex;

    public override void _Ready()
	{
        LoadEverything();
		GetTree().Root.Connect(Window.SignalName.CloseRequested, Callable.From(SaveData));
		EmitSignal("OnInitDone");
	}

	public override void _Process(double delta)
	{
		if (playing && playlist != null)
		{
			if (!player.Playing)
			{
				if (!loop) MoveSong(1);
				else
				{
					time = 0;
					player.Play(time);
					EmitSignal("OnPlay", playing);
				}
			}
			time = player.GetPlaybackPosition();
		}
	}

	public void LoadEverything()
	{
		SaveSystem.InitData(out currentPlaylist, out currentSong, out time, out volume);

		// volume
		player.VolumeDb = volume;
		volumeSlider.Value = volume;

		// load playlists
		LoadPlaylists();
		LoadPlaylist(currentPlaylist);
		if(playlist) InitSong();

		// initialize playlist displayer
		if (playlist) playlistvisualizer.LoadAllPlaylistVisuals();

		// show current playlist
		if (playlist) songsVisualizer.Load(currentPlaylist, (playlistvisualizer.container.GetChild(currentPlaylist) as PlaylistDisplay).Cover.Texture);
	}

	public void LoadPlaylists()
	{
		playlists = SaveSystem.GetAllPlaylists();
	}

	public void LoadPlaylist(int index)
	{
		currentPlaylist = index;
		playlist = playlists.Length > 0 ? SaveSystem.LoadPlaylist(playlists[currentPlaylist]) : null;
	}

	public void Play()
	{
		playing = !playing;
		if(playing) player.Play(time);
		else player.Stop();
		EmitSignal("OnPlay", playing);
	}

	public void MoveSong(int amount)
	{
		if (playlist)
		{
			currentSong += amount;
			time = 0;
			InitSong();
			playing = false;
			Play();
			EmitSignal("OnPlay", playing);
		}
	}

	public void InitSong()
	{
		int lastIndex = playlist.songs.Count - 1;
		currentSong = currentSong < 0 ? lastIndex + currentSong + 1 : currentSong;
		currentSong = currentSong > lastIndex ? currentSong - lastIndex - 1 : currentSong;

		songindex = shuffledlist == null ? currentSong : shuffledlist[currentSong];

		if (FileAccess.FileExists(playlist.songs[songindex]))
		{
			AudioStreamMP3 song = new AudioStreamMP3();
			song.Data = FileAccess.GetFileAsBytes(playlist.songs[songindex]);
			player.Stream = song;
			EmitSignal(SignalName.OnLoadSong);
		}
		else // if the file doesn't exist
		{
			GD.PrintErr($"{playlist.songs[songindex]} doesn't exist");
			playlist.songs.RemoveAt(songindex);
			playlist.Save();
			InitSong();
		}
	}

	public void EditMeta(string name, string artist, string coverpath)
	{
		if(playlist) Metadata.SetData(playlist.songs[songindex], name, artist, coverpath);
	}

	public void SaveData()
	{
		SaveSystem.SaveData(currentPlaylist, songindex, time, player.VolumeDb);
	}

	public void Shuffle() // crashes, do not use
	{
		if (!shuffle)
		{
			shuffledlist.Add(currentSong);
        }

		shuffle = !shuffle;
	}

	public bool isIntInArray(ref List<int> array, int n)
	{
		for (int i = 0; i < array.Count; i++)
		{
			if(array[i] == n) return true;
		}
		return false;
	}

	public void GetRandom()
	{
        bool goodNumb = false;
		while (!goodNumb)
		{
            int randomSong = GD.RandRange(0, playlist.songs.Count - 1);
			if(!isIntInArray(ref shuffledlist, randomSong))
			{
				shuffledlist.Add(randomSong);
				goodNumb = true;
			}
        }
    }
}
