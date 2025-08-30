using Godot;
using SSLParser;
using System;
using System.Collections.Generic;

public partial class Main : Control
{
	[Export] public AudioStreamPlayer player;
    [Export] public VideoStreamPlayer video_player;
    [Export] public Slider volumeSlider;
	[Export] public PlaylistsVisualizer playlistVisualizer;
	[Export] public SongsVisualizer songsVisualizer;

	public bool loop, playing, shuffled;

	public int current_playlist, current_song;

	public string current_playlist_path, current_song_path;

    public int current_looked_at_playlist;

    public Playlist playlist;
	public List<string> playlists;
	
	public float time, volume;

	public int offset;
	public int shuffle_index;
	public int random_offset;

	[Signal] public delegate void OnLoadSongEventHandler();
    [Signal] public delegate void OnChangePlaylistEventHandler();
    [Signal] public delegate void OnPlayEventHandler(bool playing);

	public string song
	{
		get
		{
			if(CanPlay())
				return playlist.Songs[current_song];
			else return null;
        }
	}

    public override void _Ready()
	{
        SaveSystem.GetSaveData(out current_playlist, out current_song, out time, out volume, out shuffled);
        current_looked_at_playlist = current_playlist;
		offset = current_song;

		if (shuffled)
			shuffle_index = current_song;

        // Volume
        player.VolumeDb = volume;
        volumeSlider.Value = volume;

        // Load playlists
        playlists = new List<string>(SaveSystem.GetAllPlaylists());

        if (playlists.Count > 0)
        {
            LoadPlaylist(current_playlist);
        }

        // Initialize playlist displayer
        playlistVisualizer.LoadAllPlaylistVisuals();

        if (CanPlay())
        {
            PlaySong(playlist.Songs[current_song]);
        }
        else
        {
            EmitSignal(SignalName.OnLoadSong);
        }

		// Done Loading
    }

    public override void _Process(double delta)
	{
		// Input

		if (Input.IsActionJustPressed("save")) SaveData();
		else if (Input.IsActionJustPressed("refresh")) Refresh();

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

					video_player.Play();
					video_player.StreamPosition = time;

					EmitSignal("OnPlay", playing);
				}
			}

			time = player.GetPlaybackPosition();
		}
	}

	public void CheckIndex()
	{
		if (playlists[current_playlist] != current_playlist_path)
		{
			current_playlist = playlists.IndexOf(current_playlist_path);
		}

		if (song != current_song_path)
		{
            current_song = playlist.Songs.IndexOf(current_song_path);
        }
	}

	public void LoadPlaylist(int index)
	{
		if (index < 0)
		{
			playlist = null;
			player.Stop();
			player.Stream = null;

			video_player.Stop();
			video_player.Stream = null;

			return;
		}

        current_playlist = index;
        playlist = playlists.Count > 0 ? MainParser.ParsePlaylist(playlists[current_playlist]) : null;
		current_playlist_path = playlist.Path;

		EmitSignal("OnChangePlaylist");

		// Set shuffle to be different

		GD.Randomize();
		random_offset = (int)GD.Randi();
    }

	public void Play()
	{
		playing = !playing;

		if (playing) 
		{
            player.Play(time);

			if (video_player.Stream != null)
			{
                video_player.Play();
                video_player.StreamPosition = time;
            }
        }
		else
		{
			if (video_player.Stream != null)
			{
                video_player.Stop();
            }

            player.Stop();
        }

		EmitSignal("OnPlay", playing);
	}

	public void MoveSong(int amount, bool set = false)
	{
		if (CanPlay())
		{
			if (shuffled && !set)
			{
				offset += amount;

				if (offset == shuffle_index)
				{
                    current_song = shuffle_index;
                }
				else
				{
					for(int i = 0; i < 6; i++)
					{
                        GD.Seed((ulong)(offset * 3 + random_offset + i));
                        int randomNumber = GD.RandRange(0, playlist.Songs.Count - 1);
                        if (randomNumber != current_song)
                        {
                            current_song = randomNumber;
                            offset += i / 3;
                            break;
                        }
                    }
                }
			}
			else
			{
				current_song += amount;
            }

            current_song = Mathf.Wrap(current_song, 0, playlist.Songs.Count - 1);

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
			shuffle_index = index;
			current_song = index;
            PlaySong(song);
            playing = false;
            Play();
        }
    }

	public void PlaySong(string path)
	{
		if (CanPlay())
		{
			if (player.Stream != null) 
				time = 0;

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

                VideoStreamTheora video = ConvertToGodot.GetVideo(Globals.main.song);

                video_player.Visible = video != null;
				video_player.Stream = video;

                EmitSignal("OnLoadSong");
            }
            else // if the file doesn't exist
            {
                GD.PrintErr($"{path} doesn't exist");

				if (playlist.Songs[current_song] == path)
				{
                    playlist.Songs.RemoveAt(current_song);
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

	public void SaveData() => SaveSystem.SaveData(current_playlist, current_song, time, player.VolumeDb, shuffled);

    public void Refresh()
	{
        playlists = new List<string>(SaveSystem.GetAllPlaylists());

		if (playlists.Count > 0)
			LoadPlaylist(playlists.IndexOf(playlist.Path));

		Metadata.ResetCache();

        EmitSignal(SignalName.OnLoadSong);
        playlistVisualizer.UpdatePlaylists();
    }
}