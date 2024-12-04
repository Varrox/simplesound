using Godot;

public partial class Main : Control
{
	[Export] public AudioStreamPlayer player;
	[Export] public Slider volumeSlider;
	[Export] public FileDialog importPlaylist;
	[Export] public PlaylistsVisualizer playlistvisualizer;

	public bool loop, playing;

	public int currentPlaylist;
	public Playlist playlist;
	public string[] playlists;

	public int currentSong;
	
	public float time, volume;

    [Signal] public delegate void OnLoadSongEventHandler();

    public override void _Ready()
	{
        SaveSystem.InitData(out currentPlaylist, out currentSong, out time, out volume);
		player.VolumeDb = volume;
		volumeSlider.Value = volume;
        playlists = SaveSystem.GetAllPlaylists();
		LoadPlaylist(currentPlaylist);

		playlistvisualizer.LoadAllPlaylistVisuals();

        GetTree().Root.Connect(Window.SignalName.CloseRequested, Callable.From(SaveData));
    }

	public void LoadPlaylist(int index)
	{
		currentPlaylist = index;
		playlist = SaveSystem.LoadPlaylist(playlists[currentPlaylist]);
        InitSong();
	}

    public void Play()
	{
		playing = !playing;

		if(playing) player.Play(time);

		else player.Stop();
	}

	public void SetTime(float time)
	{
		this.time = time;
		if(playing)
		{
            player.Play(time);
        }
	}

	public void EditMeta(string name, string artist, string coverpath)
	{
		Metadata.SetData(playlist.songs[currentSong], name, artist, coverpath);
	}

	public void MoveSong(int amount)
	{
		currentSong += amount;
		time = 0;
        InitSong();
		playing = false;
        Play();
    }
	
	string GetDirectory()
	{
		importPlaylist.Popup();
		return importPlaylist.CurrentPath.Remove(0, importPlaylist.CurrentPath.IndexOf('/') + 1).Replace('/', '\\');
	}

	void ClampCSong()
	{
		int lastIndex = playlist.songs.Count - 1;

        currentSong = currentSong < 0 ? lastIndex + currentSong + 1 : currentSong; // if less than zero, set it to last index
        currentSong = currentSong > lastIndex ? currentSong - lastIndex - 1 : currentSong;
    }

	public override void _Process(double delta)
	{
        if (playing)
		{
			if (!player.Playing)
			{
				if (!loop)
				{
					MoveSong(1);
				}
				else
				{
					time = 0;
                    player.Play(time);
                }
			}
			time = player.GetPlaybackPosition();
		}
    }

	public string getName()
	{
		return SaveSystem.GetName(playlist.songs[currentSong]);
    }

	public string getArtist()
	{
		return Metadata.GetArtist(playlist.songs[currentSong]);
    }

	public string getTime()
	{
		return SaveSystem.GetTimeFromSeconds(time);
	}

	public string getSongLength()
	{
		return SaveSystem.GetTimeFromSeconds((float)player.Stream.GetLength());
	}

	public float totalTime()
	{
		return (float)player.Stream.GetLength();
    }

	public ImageTexture getCover()
	{
        byte[] image = Metadata.GetCover(playlist.songs[currentSong], out string type);
        return ConvertToGodot.getCover(image, type);
    }
	
	public void InitSong()
	{
        ClampCSong();
        if(playlist.songs[currentSong].EndsWith(".mp3"))
        {
			if(FileAccess.FileExists(playlist.songs[currentSong]))
			{
                AudioStreamMP3 song = new AudioStreamMP3();
                song.Data = FileAccess.GetFileAsBytes(playlist.songs[currentSong]);
                player.Stream = song;
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
		else
		{
			GD.PrintErr($"{playlist.songs[currentSong]} is not an mp3 file, it cannot be loaded");
        }
	}

	public void SaveData()
	{
		SaveSystem.SaveData(currentPlaylist, currentSong, time, player.VolumeDb);
	}
}
