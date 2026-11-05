using Godot;
using System.Collections.Generic;

public partial class PlaylistCreator : EditorWindow {
	[Export] public StringInputField playlist_name_field;

	[Export] public Button add_track_button;
	[Export] public FileDropPanel track_drop_panel;
	[Export] public Control track_path_display_container;

	[Export] public FileInputField cover_input_field;

	[Export] public BooleanInputField album_field;
	[Export] public StringInputField artist_field;

	[Export] public Button submit_button, cancel_button;

	public List<string> tracks = new List<string>();

	bool cleared = false;

	public override void _Ready() {
		base._Ready();

		cover_input_field.filters = new (Tools.GetFilters(Constants.IMAGE_FORMATS));

		add_track_button.ButtonUp += OpenTracks;

		track_drop_panel.FilesDropped += AddTracks;

		// Submit / Cancel

		submit_button.ButtonUp += Submit;
		cancel_button.ButtonUp += Cancel;
	}

	public void Clear() {
        playlist_name_field.value = "";

        foreach (Node child in track_path_display_container.GetChildren()) {
            child.QueueFree();
        }

        tracks.Clear();

        album_field.value = false;
        artist_field.value = "";

		cleared = true;
    }

	public void Open() {
		if (!cleared)
			Clear();

        Show();
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
            string file_name = Tools.GetProperFileName(playlist_name_field.value);

			Playlist playlist = new Playlist();

            playlist.name = playlist_name_field.value;
            playlist.cover = cover_input_field.value;
            playlist.tracks = tracks;

			if (album_field.value) playlist.type = Playlist.PlaylistType.Album;

			if (artist_field.value.Trim() != "") playlist.artist = artist_field.value;

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
