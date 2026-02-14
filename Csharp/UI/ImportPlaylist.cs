using Godot;
using System.IO;

public partial class ImportPlaylist : Button
{
	[Export] public ContextMenu menu;

	Confirm sync;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ButtonDown += SetStatic;
	}

	public void SetStatic()
	{
		ConfirmationWindow confirmation_window = Globals.confirmation_window.Instantiate() as ConfirmationWindow;

		confirmation_window.message = "Import with cloud syncing enabled?";
		confirmation_window.accept_text = "Yes";
		confirmation_window.decline_text = "No";
		confirmation_window.free_on_close = true;

        GetTree().CurrentScene.AddChild(confirmation_window);

		confirmation_window.OnClose += ImportFile;
    }

	public void ImportFile(Confirm sync)
	{
		this.sync = sync;
		if (sync != Confirm.Cancelled)
		{
            Globals.SetFileDialogPlaylist();
            Globals.file_dialog.Popup();

            Globals.file_dialog.FileSelected += Import;
            Globals.file_dialog.Canceled += Cancel;
        }
    }

	public void Import(string ssl_file)
	{
		string new_path = Path.Combine(SaveSystem.USER_DATA, "Playlists", Path.GetFileName(ssl_file));

		File.Copy(ssl_file, new_path);

		Playlist playlist = Playlist.CreateFromFile(new_path);

		if (sync == Confirm.Accepted)
		{
			playlist.songs = SaveSystem.ImportSongs(playlist.songs.ToArray(), playlist.name);
			playlist.cover = SaveSystem.ImportCover(playlist.cover, playlist.name);
        }

		Globals.main.playlist_paths.Add(playlist.Save());
		Globals.main.Save();
        Globals.main.Refresh();
    }

	public void Cancel()
	{
        Globals.file_dialog.FileSelected -= Import; 
		Globals.file_dialog.Canceled -= Cancel;
		menu.opener.CloseMenu();
    }
}
