using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class Main : Control
{
	[Export] public AudioStreamPlayer audio_player;
    [Export] public VideoStreamPlayer video_player;
	
    [Export] public Player player;

	[Export] public PlaylistsVisualizer playlist_visualizer;
	[Export] public TracksVisualizer tracks_visualizer;

	public bool loop, playing, shuffled;

	public int playlist_index, track_index;

	public string current_track_path, current_share_link;

    public int looked_at_playlist;

    public Playlist playlist {
		get {
			if (playlists == null) return null;
			if (playlists.Count > playlist_index) return playlists[playlist_index];
			return null;
		}
	}

	public List<string> playlist_paths;
    public List<Playlist> playlists;

    public float time, volume;

	public int offset, shuffle_index, random_offset;

    public Action OnLoadTrack;
    public Action OnLoadPlaylist;
    public Action<bool> OnPlayingChanged;

	public string track {
		get {
			if(IsTrackAvailable()) return playlists[playlist_index].songs[track_index];
			else return null;
        }
	}

    public override void _Ready() {
		// Load Save data

		playlist_index = Globals.save_data.playlist_index;
		track_index = Globals.save_data.song_index;
		time = Globals.save_data.time;
		volume = Globals.save_data.volume;
		shuffled = Globals.save_data.shuffled;
        looked_at_playlist = Globals.save_data.looked_at_playlist;

		offset = track_index;

		if (shuffled) shuffle_index = track_index;

        // Set Volume

        audio_player.VolumeDb = volume;
        player.volume_slider.Value = volume;

        // Load playlists

        playlist_paths = Globals.save_data.playlists ?? new List<string>();
        playlists = new List<Playlist>(new Playlist[playlist_paths.Count]);

        if (Globals.save_data.playlists == null) return;

        for (int i = 0; i < playlist_paths.Count; i++) playlists[i] = Playlist.LoadFromFile(playlist_paths[i]);

        LoadPlaylist(playlist_index);

        // Initialize playlist displayer

        playlist_visualizer.LoadAllPlaylistVisuals();

        if (IsTrackAvailable()) PlayTrack(playlist.songs[track_index]);
        else OnLoadTrack?.Invoke(); // Emit anyways just so it can display no tracks

		// Done Loading
    }

    public override void _Process(double delta) {
		// Input

		if (Input.IsActionJustPressed("save")) { 
			SetSaveData();
			ApplicationManager.Save();
		}

		// Loop management

		if (playing && IsTrackAvailable()) {
			if (!audio_player.Playing) {
				if (!loop) MoveTrack(1);
				else {
					time = 0;
					audio_player.Play(time);

					video_player.Play();
					video_player.StreamPosition = time;

					OnPlayingChanged?.Invoke(playing);
				}
			}

			time = audio_player.GetPlaybackPosition();
		}
	}

	public void CheckIndex() {
		if (playlist_paths[playlist_index] != playlist.GetPath()) playlist_index = playlist_paths.IndexOf(playlist.GetPath());

		if (track != current_track_path) track_index = playlist.songs.IndexOf(current_track_path);
	}

	public void LoadPlaylist(int index) {
		if (index < 0) {
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

	public void Reshuffle() {
        GD.Randomize();
        random_offset = (int)GD.Randi();
    }

	public void Pause() {
		if (!playing)
			return;
		
		playing = false;

		if (video_player.Stream != null) video_player.Stop();
		audio_player.Stop();

		OnPlayingChanged?.Invoke(playing);
	}

	public void Play()
	{
		if (playing)
			return;
		
		playing = true;

		audio_player.Play(time);

		if (video_player.Stream != null) {
			video_player.Play();
			video_player.StreamPosition = time;
		}

		OnPlayingChanged?.Invoke(playing);
	}

	public void FlipPlayingState() {
		if (playing) {
            Pause();
        }
		else {
			Play();
        }
    }

	public void MoveTrack(int amount, bool set = false) {
		if (!IsTrackAvailable()) return;

		if (shuffled && !set) {
			offset += amount;

			if (offset == shuffle_index) track_index = shuffle_index;
			else {
				for(int i = 0; i < 6; i++) {
					GD.Seed((ulong)(offset * 3 + random_offset + i));
					int random_number = GD.RandRange(0, playlist.songs.Count);
					if (random_number != track_index) {
						track_index = random_number;
						offset += i / 3;
						break;
					}
				}
			}
		}
		else track_index += amount;

		track_index = Mathf.Wrap(track_index, 0, playlist.songs.Count);

		if(Metadata.IsFileCorrupt(track)) { // Skip if corrupted
			MoveTrack(amount);
			return;
		}

		PlayTrack(track);

		playing = false;
		FlipPlayingState();
	}

	public void SetTrack(int index) {
		if (!IsTrackAvailable()) return;
		
		offset = index;
		shuffle_index = index;
		track_index = index;

		PlayTrack(track);

		playing = false;

		FlipPlayingState();
    }

	public void PlayTrack(string path) {
		if (IsTrackAvailable()) {
			if (audio_player.Stream != null) time = 0;

            if (FileAccess.FileExists(path)) _LoadTrack(path);
            else { // if the file doesn't exist
                GD.PrintErr($"{path} doesn't exist");

				// TODO : Better missing track management needed.

				if (playlist.songs[track_index] == path) {
                    playlist.songs.RemoveAt(track_index);
                    playlist.Save();
					PlayTrack(track);
                }
            }
        }
	}

	private void _LoadTrack(in string path) {
		switch (path.GetExtension()) {
			case "mp3":
				audio_player.Stream = AudioStreamMP3.LoadFromFile(path);
				break;
			case "wav":
				audio_player.Stream = AudioStreamWav.LoadFromFile(path);
				break;
			case "ogg":
				audio_player.Stream = AudioStreamOggVorbis.LoadFromFile(path);
				break;
		}

		string video_path = Metadata.GetVideo(path);

		if (video_path != null) {
			if (FileAccess.FileExists(video_path) && video_path.EndsWith(".ogv")) {
				VideoStreamTheora video = ResourceLoader.Load(video_path) as VideoStreamTheora;

				video_player.Visible = video != null;
				video_player.Stream = video;
			}
		}

		current_share_link = Metadata.GetShareLink(path);

		OnLoadTrack?.Invoke();
	}

	public bool IsTrackAvailable() {
		if (playlists == null) return false;
		if (playlists.Count <= playlist_index) return false;
		if (playlists[playlist_index] == null) return false;
		if (playlists[playlist_index].songs == null) return false;
		if (playlists[playlist_index].songs.Count == 0) return false;
		return true;
	}

	public void SetSaveData() {
		Globals.save_data.playlist_index = playlist_index;
		Globals.save_data.song_index = track_index;
		Globals.save_data.looked_at_playlist = looked_at_playlist;
		Globals.save_data.time = time;
		Globals.save_data.volume = audio_player.VolumeDb;
		Globals.save_data.shuffled = shuffled;
		Globals.save_data.playlists = playlist_paths;
	}

    public override void _ExitTree()
    {
        SetSaveData();
    }

    public void Refresh() {
        SaveData save_data = SaveData.GetSaveData();
        playlist_paths = save_data.playlists ?? new List<string>();
        playlists = new List<Playlist>(new Playlist[playlist_paths.Count]);

        for (int i = 0; i < playlist_paths.Count; i++) playlists[i] = Playlist.LoadFromFile(playlist_paths[i]);

        LoadPlaylist(playlist_index);

        // Done Loading

        Metadata.ResetCache();

        OnLoadTrack?.Invoke();
		tracks_visualizer.Update();
        playlist_visualizer.UpdatePlaylists();
    }
}