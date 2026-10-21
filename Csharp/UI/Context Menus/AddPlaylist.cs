using System.Collections.Generic;
using System.IO;
using System.Text;
using Godot;

public partial class AddPlaylist : ContextMenuButton {
	[Export] public PackedScene create_new_playlist_template;

	public const int CREATE_NEW_PLAYLIST = 0, IMPORT_PLAYLIST_FILE = 1, IMPORT_PLAYLIST_ZIP = 2;

	public override void _Ready() {
		base._Ready();

		Texture2D plus_icon = ApplicationManager.theme.GetIcon("plus_icon", Constants.THEME_TYPE);
		Texture2D file_icon = ApplicationManager.theme.GetIcon("file_icon", Constants.THEME_TYPE);
		Texture2D zip_icon = ApplicationManager.theme.GetIcon("zip_icon", Constants.THEME_TYPE);

		GetPopup().AddIconItem(plus_icon, "Create new playlist", CREATE_NEW_PLAYLIST);
		GetPopup().AddIconItem(file_icon, "Import playlist file", IMPORT_PLAYLIST_FILE);
		GetPopup().AddIconItem(zip_icon, "Import playlist zip file", IMPORT_PLAYLIST_ZIP);
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
		if (Globals.player.Interrupt()) {
			PlaylistCreator playlist_creator = (PlaylistCreator)create_new_playlist_template.Instantiate();
			playlist_creator.Visible = false;

			GetTree().Root.AddChild(playlist_creator);

			playlist_creator.Open();
			playlist_creator.OnClose += playlist_creator.QueueFree;
		}
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

			byte[][] track_data = new byte[playlist.tracks_raw.Count][];

			for(int i = 0; i < playlist.tracks_raw.Count; i++) {
				GD.Print(playlist.tracks_raw[i]);
				if(reader.FileExists(playlist.tracks_raw[i]) && Tools.IsValidAudioFormat(playlist.tracks_raw[i])) {
					track_data[i] = reader.ReadFile(playlist.tracks_raw[i]);
				}
			}

			playlist.tracks = SaveSystem.ImportTracksData(playlist.tracks_raw.ToArray(), track_data, playlist.name);

			Globals.save_data.playlists.Add(playlist.Save());
			Globals.save_data.Save();
			
			Globals.main.Refresh();

			Finish();
		}

		Globals.file_dialog.FileSelected += AddPlaylistZIP;
        Globals.file_dialog.Canceled += Finish;
	}
}
