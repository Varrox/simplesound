using Godot;

public partial class Main : Control
{
	[Export] public AudioStreamPlayer player;
	[Export] public Slider volumeSlider;
	[Export] public PlaylistsVisualizer playlistvisualizer;
	[Export] public SongsVisualizer songsVisualizer;

	public bool loop, playing, random;

	public int currentPlaylist, currentSong;
	public string currentPlaylistPath, currentSongPath;
    public int currentLookingAtPlaylist;
    public Playlist playlist;
	public string[] playlists;
	
	public float time, volume;

	public int shuffleIndex;

	[Signal] public delegate void OnLoadSongEventHandler();
	[Signal] public delegate void OnInitDoneEventHandler();
	[Signal] public delegate void OnPlayEventHandler(bool playing);

	public string song
	{
		get
		{
			return playlist.songs[currentSong];
        }
	}

    public override void _Ready()
	{
        LoadEverything();
		GetTree().Root.CloseRequested += SaveData;
		EmitSignal("OnInitDone");
	}

	public override void _Process(double delta)
	{
		if(Input.IsKeyPressed(Key.Ctrl))
		{
			if (Input.IsKeyPressed(Key.S)) SaveData();
			else if (Input.IsKeyPressed(Key.R)) Refresh();
        }

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
		currentLookingAtPlaylist = currentPlaylist;

		// volume
		player.VolumeDb = volume;
		volumeSlider.Value = volume;

		// load playlists
		LoadPlaylists();
		LoadPlaylist(currentPlaylist);
        if (playlist) InitSong();

        // initialize playlist displayer
        if (playlist) playlistvisualizer.LoadAllPlaylistVisuals();

		// show current playlist
		if (playlist) songsVisualizer.Load(currentPlaylist, (playlistvisualizer.container.GetChild(currentPlaylist) as PlaylistDisplay).Cover.Texture);
	}

	public void LoadPlaylists()
	{
		playlists = SaveSystem.GetAllPlaylists();
	}

	public void CheckIndex()
	{
		if (playlists[currentPlaylist] != currentPlaylistPath)
		{
			currentPlaylist = SaveSystem.Find(currentPlaylistPath, ref playlists);
		}

		if (song != currentSongPath)
		{
			string[] songs = playlist.songs.ToArray();
            currentSong = SaveSystem.Find(currentSongPath, ref songs);
        }
	}

	public void LoadPlaylist(int index)
	{
		currentPlaylist = index;
		playlist = playlists.Length > 0 ? SaveSystem.LoadPlaylist(playlists[currentPlaylist]) : null;
		currentPlaylistPath = playlist.path;
    }

	public void Play()
	{
		playing = !playing;
		if(playing) player.Play(time);
		else player.Stop();
		EmitSignal("OnPlay", playing);
	}

	public void MoveSong(int amount, bool set = false)
	{
		if (playlist)
		{
			if (random && !set)
			{
				shuffleIndex += amount;
				GD.Seed((ulong)shuffleIndex);
				currentSong = GD.RandRange(0, playlist.songs.Count - 1); 
			}
			else currentSong += amount;
			time = 0;
			InitSong();
			playing = false;
			Play();
		}
	}

	public void InitSong()
	{
		int lastIndex = playlist.songs.Count - 1;
		currentSong = currentSong < 0 ? lastIndex + currentSong + 1 : currentSong;
		currentSong = currentSong > lastIndex ? currentSong - lastIndex - 1 : currentSong;

		if (FileAccess.FileExists(playlist.songs[currentSong]))
		{
			if(playlist.songs[currentSong].EndsWith(".mp3"))
			{
                AudioStreamMP3 song = new AudioStreamMP3();
                song.Data = FileAccess.GetFileAsBytes(playlist.songs[currentSong]);
                player.Stream = song;
            }
			else if(playlist.songs[currentSong].EndsWith(".wav"))
			{
				player.Stream = AudioStreamWav.LoadFromBuffer(FileAccess.GetFileAsBytes(playlist.songs[currentSong]));
            }
            else if (playlist.songs[currentSong].EndsWith(".ogg"))
            {
                player.Stream = AudioStreamOggVorbis.LoadFromBuffer(FileAccess.GetFileAsBytes(playlist.songs[currentSong]));
            }
            EmitSignal(SignalName.OnLoadSong);
		}
		else // if the file doesn't exist
		{
			GD.PrintErr($"{playlist.songs[currentSong]} doesn't exist");
			playlist.songs.RemoveAt(currentSong);
			playlist.Save();
			InitSong();
		}
	}

	public void EditMeta(string name, string artist, string coverpath, bool explicitLyrics)
	{
		if(playlist) Metadata.SetData(playlist.songs[currentSong], name, artist, coverpath, explicitLyrics);
	}

	public void SaveData()
	{
		SaveSystem.SaveData(currentPlaylist, currentSong, time, player.VolumeDb);
	}

	public void Refresh()
	{
		LoadPlaylist(currentPlaylist);
        playlistvisualizer.UpdatePlaylists();
        EmitSignal(SignalName.OnLoadSong);
    }
}
