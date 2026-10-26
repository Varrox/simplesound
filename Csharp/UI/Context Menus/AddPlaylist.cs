using System.Collections.Generic;
using System.IO;
using System.Text;
using Godot;

public partial class AddPlaylist : ContextMenuButton {
	[Export] public PackedScene create_new_playlist_template;

	public const int SEP_1 = 2;

	public const int CREATE_NEW_PLAYLIST = 0, IMPORT_PLAYLIST_FILE = 2, IMPORT_PLAYLIST_ZIP = 3;

	public override void _Ready() {
		base._Ready();

		Texture2D plus_icon = ApplicationManager.GetThemeIcon("plus_icon");
		Texture2D file_icon = ApplicationManager.GetThemeIcon("file_icon");
		Texture2D zip_icon = ApplicationManager.GetThemeIcon("zip_icon");

		GetPopup().AddIconItem(plus_icon, "Create new playlist", CREATE_NEW_PLAYLIST);
		GetPopup().SetItemTooltip(CREATE_NEW_PLAYLIST, "Opens window for creating a new playlist.");

		GetPopup().AddSeparator("", SEP_1);

		GetPopup().AddIconItem(file_icon, "Import playlist file", IMPORT_PLAYLIST_FILE);
		GetPopup().SetItemTooltip(IMPORT_PLAYLIST_FILE, $"Opens file dialog for selecting a playlist (json) file to add to {ApplicationManager.SOFTWARE_NAME}.");

		GetPopup().AddIconItem(zip_icon, "Import playlist zip file", IMPORT_PLAYLIST_ZIP);
		GetPopup().SetItemTooltip(IMPORT_PLAYLIST_ZIP, $"Opens file dialog for selecting a playlist ZIP archive (.zip file) to add to {ApplicationManager.SOFTWARE_NAME}.");
	}

    protected override void _ItemPressed(long id) {
        base._ItemPressed(id);

		switch(id) {
			case CREATE_NEW_PLAYLIST:
				_OpenPlaylistCreator();
				break;
			case IMPORT_PLAYLIST_FILE:
				_ImportPlaylistFile();
				break;
			case IMPORT_PLAYLIST_ZIP:
				_ImportPlaylistZIP();
				break;
		}
    }

	public void _OpenPlaylistCreator() {
		Globals.main.Pause();
		
		PlaylistCreator playlist_creator = (PlaylistCreator)create_new_playlist_template.Instantiate();
		playlist_creator.Visible = false;

		GetTree().Root.AddChild(playlist_creator);

		playlist_creator.Open();
		playlist_creator.OnClose += playlist_creator.QueueFree;
	}

	public void _ImportPlaylistFile() {
		Globals.SetFileDialogPlaylist();
        Globals.file_dialog.Popup();

		void Finish() {
			Globals.file_dialog.FileSelected -= AddPlaylistFile;
			Globals.file_dialog.Canceled -= Finish;
		}

		void AddPlaylistFile(string file) {
			if(!Globals.save_data.playlists.Contains(file)) {
				Globals.save_data.playlists.Add(file);
				Globals.save_data.Save();
				
				Globals.main.Refresh();
			}
			else {
				OS.Alert("Playlist could not be imported since it is already referenced in playlist library.", "Playlist import failed");
			}

			Finish();
		}

		Globals.file_dialog.FileSelected += AddPlaylistFile;
        Globals.file_dialog.Canceled += Finish;
	}

	public void _ImportPlaylistZIP() {
		Globals.SetFileDialogZIP();
        Globals.file_dialog.Popup();

		void Finish() {
			Globals.file_dialog.FileSelected -= AddPlaylistZIP;
			Globals.file_dialog.Canceled -= Finish;
		}

		void AddPlaylistZIP(string file) {
			ZipReader reader = new ZipReader();
			reader.Open(file);

			string[] files = reader.GetFiles();

			byte[] playlist_file = null;

			for(int i = 0; i < files.Length; i++) {
				if(Path.GetExtension(files[i]) == ".json") {
					playlist_file = reader.ReadFile(files[i]);
					break;
				}
			}

			if (playlist_file == null) {
				OS.Alert("Zip Archive could not be imported since no playlist .json file could be found in its contents.", "Zip import failed");
				Finish();
				return;
			}

			Playlist playlist = Playlist.LoadFromJson(playlist_file);

			if(Globals.save_data.playlists.Contains(playlist.path)) {
				OS.Alert("Playlist could not be imported since it is already referenced in playlist library.", "Playlist import failed");
				Finish();
				return;
			}

			// Cover

			if (playlist.cover != null) {
				if (reader.FileExists(playlist.cover)) {
					string cover_save_path = Path.Combine(Constants.USER_PLAYLIST_COVERS, $"{playlist.name}.{playlist.cover.GetExtension()}");
				
					File.WriteAllBytes(cover_save_path, reader.ReadFile(playlist.cover));
					playlist.cover = cover_save_path;
				}
			}

			// Tracks

			List<string> tracks = new List<string>();
			List<string> exclude = new List<string>();

			string playlist_tracks_directory = Path.Combine(Constants.USER_TRACKS, Tools.GetProperFileName(playlist.name));

			Directory.CreateDirectory(playlist_tracks_directory);

			for(int i = playlist.tracks_raw.Count - 1; i >= 0; i--) {
				if(playlist.tracks_raw[i].StartsWith("!")) { // Excludes
					string track_path = playlist.tracks_raw[i].Substring(1);

					if (reader.FileExists(track_path) && Tools.IsValidAudioFormat(track_path)) {
						exclude.Add(track_path);
						playlist.tracks_raw.RemoveAt(i);
					}
				}
			}

			for(int i = 0; i < playlist.tracks_raw.Count; i++) {
				if(reader.FileExists(playlist.tracks_raw[i]) && Tools.IsValidAudioFormat(playlist.tracks_raw[i])) {
					string destination_path = Path.Combine(playlist_tracks_directory, playlist.tracks_raw[i].GetFile());

					if (exclude.Contains(destination_path)) continue;

					File.WriteAllBytes(destination_path, reader.ReadFile(playlist.tracks_raw[i]));
					tracks.Add(destination_path);
				}
			}

			playlist.tracks = tracks;

			// Save

			Globals.save_data.playlists.Add(playlist.Save());
			Globals.save_data.Save();
			
			Globals.main.Refresh();

			Finish();
		}

		Globals.file_dialog.FileSelected += AddPlaylistZIP;
        Globals.file_dialog.Canceled += Finish;
	}
}
