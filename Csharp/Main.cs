using Godot;
using System;
using System.Collections.Generic;

public partial class Main : Control
{
	[Export] public AudioStreamPlayer audio_player;
    [Export] public VideoStreamPlayer video_player;
    [Export] public Player player;
	[Export] public PlaylistsVisualizer playlist_visualizer;
	[Export] public SongsVisualizer songs_visualizer;

	public bool loop, playing, shuffled;

	public int playlist_index, song_index;

	public string current_song_path, current_share_link;

    public int looked_at_playlist;

    public Playlist playlist
	{
		get
		{
			if (playlists != null)
			{
				if (playlists.Count > playlist_index)
				{
					return playlists[playlist_index];
                }
			}
			return null;
		}
	}

	public List<string> playlist_paths;
    public List<Playlist> playlists;

    public float time, volume;

	public int offset, shuffle_index, random_offset;

    public Action OnLoadSong;
    public Action OnLoadPlaylist;
    public Action<bool> OnPlay;

	public string song
	{
		get
		{
			if(SongAvailable()) return playlists[playlist_index].songs[song_index];
			else return null;
        }
	}

    public override void _Ready()
	{
		// Load Save data

		Globals.save_data = SaveData.GetSaveData();

		playlist_index = Globals.save_data.playlist_index;
		song_index = Globals.save_data.song_index;
		time = Globals.save_data.time;
		volume = Globals.save_data.volume;
		shuffled = Globals.save_data.shuffled;
        looked_at_playlist = Globals.save_data.looked_at_playlist;

		offset = song_index;

		if (shuffled)
			shuffle_index = song_index;

        // Set Volume

        audio_player.VolumeDb = volume;
        player.volume_slider.Value = volume;

        // Load playlists

        playlist_paths = Globals.save_data.playlists ?? new List<string>();
        playlists = new List<Playlist>(new Playlist[playlist_paths.Count]);

        if (Globals.save_data.playlists == null)
            return;

        for (int i = 0; i < playlist_paths.Count; i++)
        {
            playlists[i] = Playlist.CreateFromFile(playlist_paths[i]);
        }

        LoadPlaylist(playlist_index);

        // Initialize playlist displayer

        playlist_visualizer.LoadAllPlaylistVisuals();

        if (SongAvailable())
            PlaySong(playlist.songs[song_index]);
        else
            OnLoadSong?.Invoke(); // Emit anyways just so it can display no songs

		// Done Loading
    }

    public override void _Process(double delta)
	{
		// Input

		if (Input.IsActionJustPressed("save")) Save();

		// Loop management

		if (playing && SongAvailable())
		{
			if (!audio_player.Playing)
			{
				if (!loop) MoveSong(1);
				else
				{
					time = 0;
					audio_player.Play(time);

					video_player.Play();
					video_player.StreamPosition = time;

					OnPlay?.Invoke(playing);
				}
			}

			time = audio_player.GetPlaybackPosition();
		}
	}

	public void CheckIndex()
	{
		if (playlist_paths[playlist_index] != playlist.GetPath())
		{
			playlist_index = playlist_paths.IndexOf(playlist.GetPath());
		}

		if (song != current_song_path)
		{
            song_index = playlist.songs.IndexOf(current_song_path);
        }
	}

	public void LoadPlaylist(int index)
	{
		if (index < 0)
		{
			audio_player.Stop();
			audio_player.Stream = null;

			video_player.Stop();
			video_player.Stream = null;

			return;
		}

        playlist_index = index;

        OnLoadPlaylist?.Invoke();

		// Set shuffle to be different

		Reshuffle();
    }

	public void Reshuffle()
	{
        GD.Randomize();
        random_offset = (int)GD.Randi();
    }

	public void Play()
	{
		playing = !playing;

		if (playing) 
		{
            audio_player.Play(time);

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

            audio_player.Stop();
        }

        OnPlay?.Invoke(playing);
    }

	public void MoveSong(int amount, bool set = false)
	{
		if (SongAvailable())
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
                        int random_number = GD.RandRange(0, playlist.songs.Count);
                        if (random_number != song_index)
                        {
                            song_index = random_number;
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

            song_index = Mathf.Wrap(song_index, 0, playlist.songs.Count);

			if(Metadata.IsFileCorrupt(song)) // Skip if corrupted
			{
				MoveSong(amount);
				return;
			}
				

            PlaySong(song);

			playing = false;
			Play();
		}
	}

	public void SetSong(int index)
	{
		if (SongAvailable())
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
		if (SongAvailable())
		{
			if (audio_player.Stream != null) 
				time = 0;

            if (FileAccess.FileExists(path))
            {
                if (path.EndsWith(".mp3"))
                {
                    audio_player.Stream = AudioStreamMP3.LoadFromBuffer(FileAccess.GetFileAsBytes(path));
                }
                else if (path.EndsWith(".wav"))
                {
                    audio_player.Stream = AudioStreamWav.LoadFromBuffer(FileAccess.GetFileAsBytes(path));
                }
                else if (path.EndsWith(".ogg"))
                {
                    audio_player.Stream = AudioStreamOggVorbis.LoadFromBuffer(FileAccess.GetFileAsBytes(path));
                }

                VideoStreamTheora video = ConvertToGodot.GetVideoFromFile(Globals.main.song);

                video_player.Visible = video != null;
				video_player.Stream = video;

                current_share_link = Metadata.GetShareLink(path);

                OnLoadSong?.Invoke();
            }
            else // if the file doesn't exist
            {
                GD.PrintErr($"{path} doesn't exist");

				if (playlist.songs[song_index] == path)
				{
                    playlist.songs.RemoveAt(song_index);
                    playlist.Save();
					PlaySong(song);
                }
            }
        }
	}

	public bool SongAvailable()
	{
		if(playlists != null)
			if (playlists.Count > playlist_index)
				if (playlists[playlist_index] != null)
					if (playlists[playlist_index].songs != null)
						if (playlists[playlist_index].songs.Count > 0) return true;
		return false;
	}

	public void Save() 
	{
		Globals.save_data.playlist_index = playlist_index;
		Globals.save_data.song_index = song_index;
		Globals.save_data.looked_at_playlist = looked_at_playlist;
		Globals.save_data.time = time;
		Globals.save_data.volume = audio_player.VolumeDb;
		Globals.save_data.shuffled = shuffled;
		Globals.save_data.playlists = playlist_paths;

        Globals.save_data.Save();
	}

    public void Refresh()
	{
        SaveData save_data = SaveData.GetSaveData();
        playlist_paths = save_data.playlists ?? new List<string>();
        playlists = new List<Playlist>(new Playlist[playlist_paths.Count]);

        for (int i = 0; i < playlist_paths.Count; i++)
        {
            playlists[i] = Playlist.CreateFromFile(playlist_paths[i]);
        }

        LoadPlaylist(playlist_index);

        // Done Loading

        Metadata.ResetCache();

        OnLoadSong?.Invoke();
        playlist_visualizer.UpdatePlaylists();
    }
}