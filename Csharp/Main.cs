using Godot;
using SSLParser;
using System;
using System.Collections.Generic;

public partial class Main : Control
{
	[Export] public AudioStreamPlayer player;
    [Export] public VideoStreamPlayer video_player;
    [Export] public Slider volumeSlider;
	[Export] public PlaylistsVisualizer playlist_visualizer;
	[Export] public SongsVisualizer songs_visualizer;

	public bool loop, playing, shuffled;

	public int playlist_index, song_index;

	public string current_playlist_path, current_song_path, current_share_link;

    public int current_looked_at_playlist;

    public Playlist playlist;
	public List<string> playlists;
	
	public float time, volume;

	public int offset, shuffle_index, random_offset;

    public Action OnLoadSong;
    public Action OnChangePlaylist;
    public Action<bool> OnPlay;

	public string song
	{
		get
		{
			if(CanPlay())
				return playlist.Songs[song_index];
			else return null;
        }
	}

    public override void _Ready()
	{
		// Load Save data

		GD.Print(ParsingTools.StringifyArray(new[] { new[]{"Sigma", "Rizz" }, new[] { "Gooning", "Rizzing", "Ohio" } }));

		List<List<string>> ar = ParsingTools.ParseArray<List<string>>("[[\"Sigma\", \"Rizz\"], [\"Gooning\", \"Rizzing\", \"Ohio\"]]");

		for (int i = 0; i < ar.Count; i++)
		{
			for (int j = 0; j < ar[i].Count; j++)
			{
				GD.Print(ar[i][j]);
			}
		}

        SaveData save_data = SaveSystem.GetSaveData();

		playlist_index = save_data.playlist_index;
		song_index = save_data.song_index;
		time = save_data.time;
		volume = save_data.volume;
		shuffled = save_data.shuffled;

        current_looked_at_playlist = playlist_index;

		offset = song_index;

		if (shuffled)
			shuffle_index = song_index;

        // Set Volume

        player.VolumeDb = volume;
        volumeSlider.Value = volume;

        // Load playlists

        playlists = new List<string>(SaveSystem.GetAllPlaylists());

        if (playlists.Count > 0)
            LoadPlaylist(playlist_index);

        // Initialize playlist displayer

        playlist_visualizer.LoadAllPlaylistVisuals();

        if (CanPlay())
            PlaySong(playlist.Songs[song_index]);
        else
            OnLoadSong?.Invoke(); // Emit anyways just so it can display no songs

		// Done Loading
    }

    public override void _Process(double delta)
	{
		// Input

		if (Input.IsActionJustPressed("save")) SaveData();

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

					OnPlay?.Invoke(playing);
				}
			}

			time = player.GetPlaybackPosition();
		}
	}

	public void CheckIndex()
	{
		if (playlists[playlist_index] != current_playlist_path)
		{
			playlist_index = playlists.IndexOf(current_playlist_path);
		}

		if (song != current_song_path)
		{
            song_index = playlist.Songs.IndexOf(current_song_path);
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

        playlist_index = index;
        playlist = playlists.Count > 0 ? MainParser.ParsePlaylist(playlists[playlist_index]) : null;
		current_playlist_path = playlist.Path;

        OnChangePlaylist?.Invoke();

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

        OnPlay?.Invoke(playing);
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
                    song_index = shuffle_index;
                }
				else
				{
					for(int i = 0; i < 6; i++)
					{
                        GD.Seed((ulong)(offset * 3 + random_offset + i));
                        int randomNumber = GD.RandRange(0, playlist.Songs.Count - 1);
                        if (randomNumber != song_index)
                        {
                            song_index = randomNumber;
                            offset += i / 3;
                            break;
                        }
                    }
                }
			}
			else
			{
				song_index += amount;
            }

            song_index = Mathf.Wrap(song_index, 0, playlist.Songs.Count - 1);

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
			song_index = index;
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

                current_share_link = Metadata.GetShareLink(path);

                OnLoadSong?.Invoke();
            }
            else // if the file doesn't exist
            {
                GD.PrintErr($"{path} doesn't exist");

				if (playlist.Songs[song_index] == path)
				{
                    playlist.Songs.RemoveAt(song_index);
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

	public void SaveData() => new SaveData {playlist_index = playlist_index, song_index = song_index, time = time, volume = player.VolumeDb, shuffled = shuffled }.Save();

    public void Refresh()
	{
        playlists = new List<string>(SaveSystem.GetAllPlaylists());

		if (playlists.Count > 0)
			LoadPlaylist(playlists.IndexOf(playlist.Path));

		Metadata.ResetCache();

        OnLoadSong?.Invoke();
        playlist_visualizer.UpdatePlaylists();
    }
}