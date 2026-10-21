using Godot;

public partial class AddPlaylist : ContextMenuButton {
	[Export] public PackedScene create_new_playlist_template;

	public const int CREATE_NEW_PLAYLIST = 0, IMPORT_PLAYLIST_FILE = 1, IMPORT_PLAYLIST_ZIP = 2;

	public override void _Ready() {
		base._Ready();

		Texture2D plus_icon = ApplicationManager.theme.GetIcon("plus_icon", Constants.THEME_TYPE);
		Texture2D file_icon = ApplicationManager.theme.GetIcon("file_icon", Constants.THEME_TYPE);
		//Texture2D zip_icon = ApplicationManager.theme.GetIcon("zip_icon", Constants.THEME_TYPE);

		GetPopup().AddIconItem(plus_icon, "Create new playlist", CREATE_NEW_PLAYLIST);
		GetPopup().AddIconItem(file_icon, "Import playlist file", IMPORT_PLAYLIST_FILE);
		//GetPopup().AddIconItem(zip_icon, "Import playlist zip file", IMPORT_PLAYLIST_ZIP);
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
}
