using Godot;
using System.Collections.Generic;

public partial class PlaylistCreator : EditorWindow {
	[Export] public ThemeLineEdit playlist_name_field;

	[Export] public Button add_track_button;
	[Export] public FileDropPanel track_drop_panel;
	[Export] public Control track_path_display_container;

	[Export] public Button add_cover_button;
	[Export] public PathDisplay cover_path_display;

	[Export] public CheckBox album_field;
	[Export] public ThemeLineEdit artist_field;

	[Export] public Button submit_button, cancel_button;

	public List<string> tracks = new List<string>();
	public string cover_path;

	bool cleared = false;

	public override void _Ready() {
		base._Ready();

		add_cover_button.ButtonUp += OpenCover;
		cover_path_display.delete.ButtonUp += ClearCover;

		add_track_button.ButtonUp += OpenTracks;

		track_drop_panel.FilesDropped += AddTracks;

		// Submit / Cancel

		submit_button.ButtonUp += Submit;
		cancel_button.ButtonUp += Cancel;
	}

	public void Clear() {
        playlist_name_field.Text = "";

        foreach (Node child in track_path_display_container.GetChildren()) {
            child.QueueFree();
        }

        tracks.Clear();

        album_field.ButtonPressed = false;
        artist_field.Text = "";

		cleared = true;
    }

	public void Open() {
		if (!cleared)
			Clear();

        Show();
	}

    public void OpenCover() {
		ThemeFileDialog file_dialog = new ThemeFileDialog();

		AddChild(file_dialog);

		file_dialog.SetCover();
		file_dialog.Popup();

		void SetCover(string path) {
			cover_path = path;
			cover_path_display.path = cover_path;

			Finish();
		}

		void Finish() {
            file_dialog.FileSelected -= SetCover;
			file_dialog.Canceled -= Finish;

			file_dialog.QueueFree();
        }

		file_dialog.FileSelected += SetCover;
        file_dialog.Canceled += Finish;
    }
	
	public void ClearCover() {
		cover_path = "";
        cover_path_display.path = cover_path;
    }

	public void OpenTracks() {
		ThemeFileDialog file_dialog = new ThemeFileDialog();

		AddChild(file_dialog);

		file_dialog.SetTracks();
		file_dialog.Popup();

		void FileDialogAddTracks(string[] paths) {
			AddTracks(paths);

			Finish();
		}

		void Finish() {
            file_dialog.FilesSelected -= FileDialogAddTracks;
			file_dialog.Canceled -= Finish;

			file_dialog.QueueFree();
        }

        file_dialog.FilesSelected += FileDialogAddTracks;
        file_dialog.Canceled += Finish;
    }

    public void AddTracks(string[] paths) {
		foreach(string path in paths) {
			if(Tools.ValidAudioFile(path)) {
				if(!tracks.Contains(path)) {
					PathDisplay disp = new PathDisplay();
					disp.is_file = true;

					track_path_display_container.AddChild(disp);

					disp.path = path;

					tracks.Add(path);

					disp.delete.ButtonUp += () => {tracks.Remove(path); disp.QueueFree();};
				}
				else {
					GD.PushError($"{path} is already in this playlist.");
				}
			}
			else {
                GD.PushError($"{path} is not a valid audio file, it cannot be added to this playlist.");
			}
		}
    }

	public void Submit() {
		Hide();

		if(!cancelled) {
            string file_name = Tools.GetProperFileName(playlist_name_field.Text);

			Playlist playlist = new Playlist();

            playlist.name = playlist_name_field.Text;
            playlist.cover = cover_path;
            playlist.tracks = tracks;

			if (album_field.ButtonPressed) playlist.type = Playlist.PlaylistType.Album;

			if (artist_field.Text.Trim() != "") playlist.artist = artist_field.Text;

            Clear();

			Main.playlist_paths.Add(playlist.Save());
			Main.instance.SetSaveData();
            Main.instance.Refresh();
		}

        EmitSignal(SignalName.OnClose);
    }
	public void Cancel() {
		cancelled = true;
		Submit();
	}
}
