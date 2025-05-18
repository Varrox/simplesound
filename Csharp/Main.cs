using Godot;
using SSLParser;
using System;

public partial class Main : Control
{
	[Export] public AudioStreamPlayer player;
	[Export] public Slider volumeSlider;
	[Export] public PlaylistsVisualizer playlistVisualizer;
	[Export] public SongsVisualizer songsVisualizer;

	public bool loop, playing, random;

	public int currentPlaylist, currentSong;

	public string currentPlaylistPath, currentSongPath;

    public int currentLookingAtPlaylist;

    public Playlist playlist;
	public string[] playlists;
	
	public float time, volume;

	public int offset;
	public int shuffleIndex;
	public int randomOffset;

	[Signal] public delegate void OnLoadSongEventHandler();
    [Signal] public delegate void OnChangePlaylistEventHandler();
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
        SaveSystem.GetSaveData(out currentPlaylist, out currentSong, out time, out volume);
        currentLookingAtPlaylist = currentPlaylist;
		offset = currentSong;

        // Volume
        player.VolumeDb = volume;
        volumeSlider.Value = volume;

        // Load playlists
        playlists = SaveSystem.GetAllPlaylists();

        if (playlists.Length > 0)
        {
            LoadPlaylist(currentPlaylist);
        }

        // Initialize playlist displayer
        playlistVisualizer.LoadAllPlaylistVisuals();

        if (CanPlay())
        {
            PlaySong(playlist.Songs[currentSong]);

            // Show current playlist
            songsVisualizer.Load(currentPlaylist, (playlistVisualizer.container.GetChild(currentPlaylist) as PlaylistDisplay).Cover.Texture);
        }
        else
        {
            EmitSignal(SignalName.OnLoadSong);
        }
    }

    public override void _Process(double delta)
	{
		// Input

		if (Input.IsActionJustPressed("save")) SaveData();
		else if (Input.IsActionJustPressed("refresh")) Refresh();
		else if (Input.IsActionJustPressed("scale_up")) GetTree().Root.ContentScaleFactor += 0.1f;
        else if (Input.IsActionJustPressed("scale_down")) GetTree().Root.ContentScaleFactor -= 0.1f;

        GetTree().Root.ContentScaleFactor = (float)Mathf.Clamp((double)GetTree().Root.ContentScaleFactor, 0.5, 1.5);

		// Loop management

		if (playing && CanPlay())
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

	public void CheckIndex()
	{
		if (playlists[currentPlaylist] != currentPlaylistPath)
		{
			currentPlaylist = Array.IndexOf(playlists, currentPlaylistPath);
		}

		if (song != currentSongPath)
		{
            currentSong = playlist.Songs.IndexOf(currentSongPath);
        }
	}

	public void LoadPlaylist(int index)
	{
		if (index < 0)
		{
			playlist = null;
			player.Stop();
			player.Stream = null;
			return;
		}

        currentPlaylist = index;
        playlist = playlists.Length > 0 ? MainParser.ParsePlaylist(playlists[currentPlaylist]) : null;
		currentPlaylistPath = playlist.Path;

		EmitSignal("OnChangePlaylist");

		// Set shuffle to be different

		GD.Randomize();
		randomOffset = (int)GD.Randi();
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
				offset += amount;

				if (offset == shuffleIndex)
				{
                    currentSong = shuffleIndex;
                }
				else
				{
					for(int i = 0; i < 6; i++)
					{
                        GD.Seed((ulong)(offset * 3 + randomOffset + i));
                        int randomNumber = GD.RandRange(0, playlist.Songs.Count - 1);
                        if (randomNumber != currentSong)
                        {
                            currentSong = randomNumber;
                            offset += i / 3;
                            break;
                        }
                    }
                }
			}
			else
			{
				currentSong += amount;
            }

			PlaySong(song);

			playing = false;
			Play();
		}
	}

	public void SetSong(int index)
	{
		if (CanPlay())
		{
			offset = index;
			shuffleIndex = index;
			currentSong = index;
            PlaySong(song);
            playing = false;
            Play();
        }
    }

	public void PlaySong(string path)
	{
		if (CanPlay())
		{
            time = 0;
            currentSong = Tools.Wrap(currentSong, 0, playlist.Songs.Count - 1);

            if (FileAccess.FileExists(path))
            {
                if (path.EndsWith(".mp3"))
                {
                    player.Stream = AudioStreamMP3.LoadFromBuffer(FileAccess.GetFileAsBytes(path));
                }
                else if (path.EndsWith(".wav"))
                {
                    player.Stream = AudioStreamWav.LoadFromBuffer(FileAccess.GetFileAsBytes(path));
                }
                else if (path.EndsWith(".ogg"))
                {
                    player.Stream = AudioStreamOggVorbis.LoadFromBuffer(FileAccess.GetFileAsBytes(path));
                }

                EmitSignal("OnLoadSong");
            }
            else // if the file doesn't exist
            {
                GD.PrintErr($"{path} doesn't exist");

				if (playlist.Songs[currentSong] == path)
				{
                    playlist.Songs.RemoveAt(currentSong);
                    playlist.Save();
					PlaySong(song);
                }
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

	public void SaveData() => SaveSystem.SaveData(currentPlaylist, currentSong, time, player.VolumeDb);

    public void Refresh()
	{
        playlists = SaveSystem.GetAllPlaylists();

		if (playlists.Length > 0)
			LoadPlaylist(Array.IndexOf(playlists, playlist.GetPath()));

		Metadata.ResetCache();

        EmitSignal(SignalName.OnLoadSong);
        playlistVisualizer.UpdatePlaylists();
    }
}