using Godot;
using System.Collections.Generic;

[GlobalClass]
public partial class Main : Control {
	[Export] public AudioStreamPlayer audio_player;
    [Export] public VideoStreamPlayer video_player;

	[Export] public PlaylistsVisualizer playlist_visualizer;
	[Export] public TracksVisualizer tracks_visualizer;

	public static Main instance;

	public static bool loop, playing, shuffled;

	public static int playlist_index, track_index, looked_at_playlist;

	public string current_track_path;

	private string playing_path;

	public static List<string> playlist_paths;
    public static List<Playlist> playlists;

    public static float time, volume;

	public static int offset, shuffle_index, random_offset;

	[Signal]
    public delegate void OnLoadTrackEventHandler();

	[Signal]
    public delegate void OnLoadPlaylistEventHandler();

	/// Emitted when played or paused.
	[Signal]
    public delegate void OnPlayingChangedEventHandler();

	public static string GetCurrentTrack() {
		if(IsTrackAvailable()) return playlists[playlist_index].tracks[track_index];
		else return null;
	}

	public static Playlist GetCurrentPlaylist() {
		if (playlists == null) return null;
		if (playlists.Count > playlist_index) return playlists[playlist_index];
		return null;
	}

    public Main() {
        instance = this;
    }

    public override void _Ready() {
		// Load Save data

		playlist_index = ApplicationManager.save_data.playlist_index;
		track_index = ApplicationManager.save_data.track_index;
		time = ApplicationManager.save_data.time;
		volume = ApplicationManager.save_data.volume;
		shuffled = ApplicationManager.save_data.shuffled;
        looked_at_playlist = ApplicationManager.save_data.looked_at_playlist;

		offset = track_index;

		if (shuffled) shuffle_index = track_index;

        // Set Volume

        audio_player.VolumeDb = volume;
        Player.instance.volume_slider.Value = volume;

        // Load playlists

        playlist_paths = ApplicationManager.save_data.playlists ?? new List<string>();
        playlists = new List<Playlist>(new Playlist[playlist_paths.Count]);

        if (ApplicationManager.save_data.playlists == null) return;

        for (int i = 0; i < playlist_paths.Count; i++) playlists[i] = Playlist.LoadFromFile(playlist_paths[i]);

        LoadPlaylist(playlist_index);

        // Initialize playlist displayer

        playlist_visualizer.LoadAllPlaylistVisuals();

        if (IsTrackAvailable()) PlayTrack(GetCurrentPlaylist().tracks[track_index]);
        else EmitSignal(SignalName.OnLoadTrack); // Emit anyways just so it can display no tracks

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

					EmitSignal(SignalName.OnPlayingChanged);
				}
			}

			time = audio_player.GetPlaybackPosition();
		}
	}

	public void CheckIndex() {
		if (playlist_paths[playlist_index] != GetCurrentPlaylist().path) playlist_index = playlist_paths.IndexOf(GetCurrentPlaylist().path);

		if (GetCurrentTrack() != current_track_path) track_index = GetCurrentPlaylist().tracks.IndexOf(current_track_path);
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

        EmitSignal(SignalName.OnLoadPlaylist);

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

		EmitSignal(SignalName.OnPlayingChanged);
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

		EmitSignal(SignalName.OnPlayingChanged);
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
					int random_number = GD.RandRange(0, GetCurrentPlaylist().tracks.Count);
					if (random_number != track_index) {
						track_index = random_number;
						offset += i / 3;
						break;
					}
				}
			}
		}
		else track_index += amount;

		track_index = Mathf.Wrap(track_index, 0, GetCurrentPlaylist().tracks.Count);

		int total_skips = 0;
		while(Metadata.IsFileCorrupt(GetCurrentTrack())) { // Skip if corrupted
			track_index = Mathf.Wrap(track_index + 1, 0, GetCurrentPlaylist().tracks.Count);
			total_skips++;

			if (total_skips >= GetCurrentPlaylist().tracks.Count) {
				return;
			}
		}

		PlayTrack(GetCurrentTrack());

		playing = false;
		Play();
	}

	public void SetTrack(int index) {
		if (!IsTrackAvailable()) return;

		index = Mathf.Clamp(index, 0, GetCurrentPlaylist().tracks.Count);

		int total_skips = 0;
		while(Metadata.IsFileCorrupt(GetCurrentPlaylist().tracks[index])) { // Skip if corrupted
			index = Mathf.Wrap(index + 1, 0, GetCurrentPlaylist().tracks.Count);
			total_skips++;

			if (total_skips >= GetCurrentPlaylist().tracks.Count) {
				return;
			}
		}

		offset = index;
		shuffle_index = index;
		track_index = index;

		PlayTrack(GetCurrentTrack());

		playing = false;
		Play();
    }

	public void PlayTrack(string path) {
		if (IsTrackAvailable()) {
			if (audio_player.Stream != null) time = 0;

            if (FileAccess.FileExists(path)) _LoadTrack(path);
            else { // if the file doesn't exist
                GD.PrintErr($"{path} doesn't exist");
            }
        }
	}

	private void _LoadTrack(in string path) {
		playing_path = path;

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

		EmitSignal(SignalName.OnLoadTrack);
	}

	public static bool IsTrackAvailable() {
		if (playlists == null) return false;
		if (playlists.Count <= playlist_index) return false;
		if (playlists[playlist_index] == null) return false;
		if (playlists[playlist_index].tracks == null) return false;
		if (playlists[playlist_index].tracks.Count == 0) return false;
		return true;
	}

	public void SetSaveData() {
		ApplicationManager.save_data.playlist_index = playlist_index;
		ApplicationManager.save_data.track_index = track_index;
		ApplicationManager.save_data.looked_at_playlist = looked_at_playlist;
		ApplicationManager.save_data.time = time;
		ApplicationManager.save_data.volume = audio_player.VolumeDb;
		ApplicationManager.save_data.shuffled = shuffled;
		ApplicationManager.save_data.playlists = playlist_paths;
	}

    public override void _ExitTree() {
        SetSaveData();
    }

    public void Refresh() {
		string current_playlist = playlist_paths[playlist_index];
		int current_playlist_index = playlist_index;

        SaveData save_data = SaveData.GetSaveData();

        playlist_paths = save_data.playlists ?? new List<string>();
		
        playlists = new List<Playlist>();

        for (int i = 0; i < playlist_paths.Count; i++) playlists.Add(Playlist.LoadFromFile(playlist_paths[i]));

		int target_playlist;

		if (!playlist_paths.Contains(current_playlist)) {
			target_playlist = Mathf.Clamp(current_playlist_index, 0, playlist_paths.Count - 1);
		}
		else {
			target_playlist = playlist_paths.IndexOf(current_playlist);
		}

        LoadPlaylist(target_playlist);

		if (!GetCurrentPlaylist().tracks.Contains(playing_path)) {

			PlayTrack(GetCurrentTrack());
			Pause();
		}
		else {
			track_index = GetCurrentPlaylist().tracks.IndexOf(playing_path);
		}

        // Done Loading

        Metadata.ResetCache();

        EmitSignal(SignalName.OnLoadTrack);
        playlist_visualizer.UpdatePlaylists();
    }
}