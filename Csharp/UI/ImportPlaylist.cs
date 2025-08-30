using Godot;
using SSLParser;
using System.IO;

public partial class ImportPlaylist : Button
{
	[Export] ContextMenu menu;

	Confirm sync;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ButtonDown += SetStatic;
	}

	public void SetStatic()
	{
		ConfirmationWindow confirmationWindow = Globals.confirmation_window.Instantiate() as ConfirmationWindow;

		confirmationWindow.Message = "Import with cloud syncing enabled?";
		confirmationWindow.AcceptText = "Yes";
		confirmationWindow.DeclineText = "No";
		confirmationWindow.free_on_close = true;

        GetTree().CurrentScene.AddChild(confirmationWindow);

		confirmationWindow.OnClose += ImportFile;
    }

	public void ImportFile(Confirm sync)
	{
		this.sync = sync;
		if (sync != Confirm.Cancelled)
		{
            Globals.SetFileDialogPlaylist();
            Globals.file_dialog.Popup();

            Globals.file_dialog.FileSelected += ImportSSL;
            Globals.file_dialog.Canceled += Cancel;
        }
    }

	public void ImportSSL(string ssl_file)
	{
		string new_path = Path.Combine(SaveSystem.USER_DATA, "Playlists", Path.GetFileName(ssl_file));

		File.Copy(ssl_file, new_path);

		Playlist playlist = MainParser.ParsePlaylist(new_path);

		if (sync == Confirm.Accepted)
		{
			playlist.Songs = SaveSystem.ImportSongs(playlist.Songs.ToArray(), playlist.Name);
			playlist.Cover = SaveSystem.ImportCover(playlist.Cover, playlist.Name);
        }

		Globals.main.playlists.Add(playlist.Save());
        SaveSystem.SaveAllPlaylists(Globals.main.playlists.ToArray());
        Globals.main.Refresh();
    }

	public void Cancel()
	{
        Globals.file_dialog.FileSelected -= ImportSSL; 
		Globals.file_dialog.Canceled -= Cancel;
		menu.CloseMenu();
    }
}
