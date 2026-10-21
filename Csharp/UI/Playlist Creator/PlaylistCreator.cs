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
        cover_path_display.SetPath();

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

        Globals.file_dialog.Reparent(this);

        Show();
	}

    public void OpenCover() {
        Globals.SetFileDialogCover();
        Globals.file_dialog.Popup();

		Globals.file_dialog.FileSelected += SetCover;
        Globals.file_dialog.Canceled += CancelSetCover;
    }

	public void SetCover(string path) {
		cover_path = path;
		cover_path_display.SetPath(cover_path);
        CancelSetCover();
    }
	
	public void ClearCover() {
		cover_path = "";
        cover_path_display.SetPath(cover_path);
    }

	public void OpenTracks() {
		Globals.SetFileDialogTracks();
		Globals.file_dialog.Popup();

        Globals.file_dialog.FilesSelected += FileDialogAddTracks;
        Globals.file_dialog.Canceled += CancelAddTracks;
    }

	void CancelAddTracks() { Globals.file_dialog.FilesSelected -= FileDialogAddTracks; Globals.file_dialog.Canceled -= CancelAddTracks; }
    void CancelSetCover() { Globals.file_dialog.FileSelected -= SetCover; Globals.file_dialog.Canceled -= CancelSetCover; }

    public void AddTracks(string[] paths) {
		foreach(string path in paths) {
			if(Tools.ValidAudioFile(path)) {
				if(!tracks.Contains(path)) {
					var disp = Globals.path_display.Instantiate() as PathDisplay;
					disp.is_file = true;

					track_path_display_container.AddChild(disp);

					disp.SetPath(path);

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

	public void FileDialogAddTracks(string[] paths) {
		AddTracks(paths);
		CancelAddTracks();
	}

	public void Submit() {
		Hide();

        Globals.file_dialog.Reparent(Globals.self);

		if(!cancelled) {
            string file_name = Tools.GetProperFileName(playlist_name_field.Text);

			Playlist playlist = new Playlist();

            playlist.name = playlist_name_field.Text;
            playlist.cover = cover_path;
            playlist.tracks = tracks;

			if (album_field.ButtonPressed) playlist.type = Playlist.PlaylistType.Album;

			if (artist_field.Text.Trim() != "") playlist.artist = artist_field.Text;

            Clear();

			Globals.main.playlist_paths.Add(playlist.Save());
			Globals.main.SetSaveData();
            Globals.main.Refresh();
		}

		Globals.player.interrupted = false;

        EmitSignal(SignalName.OnClose);
    }
	public void Cancel() {
		cancelled = true;
		Submit();
	}
}
