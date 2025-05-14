using Godot;
using SSLParser;

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
	[Signal] public delegate void OnPlayEventHandler(bool playing);

	public string song
	{
		get
		{
			if(CanPlay())
				return playlist.Songs[currentSong];
			else return null;
        }
	}

    public override void _Ready()
	{
        GetTree().Root.CloseRequested += SaveData;

        LoadEverything();
	}

	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("save")) SaveData();
		else if (Input.IsActionJustPressed("refresh")) Refresh();
		else if (Input.IsActionJustPressed("scale_up")) GetTree().Root.ContentScaleFactor += 0.1f;
        else if (Input.IsActionJustPressed("scale_down")) GetTree().Root.ContentScaleFactor -= 0.1f;

        GetTree().Root.ContentScaleFactor = (float)Mathf.Clamp((double)GetTree().Root.ContentScaleFactor, 0.5, 1.5);

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

		if (playlists.Length > 0)
		{
            LoadPlaylist(currentPlaylist);
        }

		if(CanPlay())
		{
            InitSong();

            // show current playlist
            songsVisualizer.Load(currentPlaylist, (playlistvisualizer.container.GetChild(currentPlaylist) as PlaylistDisplay).Cover.Texture);
        }
		else
		{
            EmitSignal(SignalName.OnLoadSong);
        }

        // initialize playlist displayer
        playlistvisualizer.LoadAllPlaylistVisuals();
    }

	public void LoadPlaylists()
	{
		playlists = SaveSystem.GetAllPlaylists();
	}

	public void CheckIndex()
	{
		if (playlists[currentPlaylist] != currentPlaylistPath)
		{
			currentPlaylist = Tools.FindString(currentPlaylistPath, ref playlists);
		}

		if (song != currentSongPath)
		{
			string[] songs = playlist.Songs.ToArray();
            currentSong = Tools.FindString(currentSongPath, ref songs);
        }
	}

	public void LoadPlaylist(int index)
	{
		currentPlaylist = index;
		playlist = playlists.Length > 0 ? MainParser.ParsePlaylist(playlists[currentPlaylist]) : null;
		currentPlaylistPath = playlist.Path;
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
		if (CanPlay())
		{
			if (random && !set)
			{
				shuffleIndex += amount;
				GD.Seed((ulong)shuffleIndex);
				currentSong = GD.RandRange(0, playlist.Songs.Count - 1); 
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
		if (CanPlay())
		{
            int lastIndex = playlist.Songs.Count - 1;
            currentSong = currentSong < 0 ? lastIndex + currentSong + 1 : currentSong;
            currentSong = currentSong > lastIndex ? currentSong - lastIndex - 1 : currentSong;

            if (FileAccess.FileExists(playlist.Songs[currentSong]))
            {
                if (playlist.Songs[currentSong].EndsWith(".mp3"))
                {
                    AudioStreamMP3 song = new AudioStreamMP3();
                    song.Data = FileAccess.GetFileAsBytes(playlist.Songs[currentSong]);
                    player.Stream = song;
                }
                else if (playlist.Songs[currentSong].EndsWith(".wav"))
                {
                    player.Stream = AudioStreamWav.LoadFromBuffer(FileAccess.GetFileAsBytes(playlist.Songs[currentSong]));
                }
                else if (playlist.Songs[currentSong].EndsWith(".ogg"))
                {
                    player.Stream = AudioStreamOggVorbis.LoadFromBuffer(FileAccess.GetFileAsBytes(playlist.Songs[currentSong]));
                }
                EmitSignal(SignalName.OnLoadSong);
            }
            else // if the file doesn't exist
            {
                GD.PrintErr($"{playlist.Songs[currentSong]} doesn't exist");
                playlist.Songs.RemoveAt(currentSong);
                playlist.Save();
                InitSong();
            }
        }
	}

	public bool CanPlay()
	{
		if (playlist)
			if (playlist.Songs != null)
				if (playlist.Songs.Count > 0) return true;
		return false;
	}

	public void EditMeta(string name, string artist, string coverpath, bool explicitLyrics)
	{
		if(playlist) Metadata.SetData(playlist.Songs[currentSong], name, artist, coverpath, explicitLyrics);
	}

	public void SaveData()
	{
		SaveSystem.SaveData(currentPlaylist, currentSong, time, player.VolumeDb);
	}

    public void Refresh()
	{
		LoadPlaylists();
		
		if (playlists.Length > 0) 
			LoadPlaylist(Tools.FindString(playlist.GetPath(), ref playlists));

        EmitSignal(SignalName.OnLoadSong);
        playlistvisualizer.UpdatePlaylists();
    }
}
