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
		confirmationWindow.freeOnClose = true;

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
		string newPath = Path.Combine(SaveSystem.UserData, "Playlists", Path.GetFileName(ssl_file));

		File.Copy(ssl_file, newPath);

		Playlist playlist = MainParser.ParsePlaylist(newPath);

		if (sync == Confirm.Accepted)
		{
			playlist.Songs = SaveSystem.ImportSongs(playlist.Songs.ToArray(), playlist.Name);
			playlist.Cover = SaveSystem.ImportCover(playlist.Cover, playlist.Name);
        }

        Tools.AddToArray(ref Globals.main.playlists, playlist.Save());
        SaveSystem.SaveAllPlaylists(Globals.main.playlists);
        Globals.main.Refresh();
    }

	public void Cancel()
	{
        Globals.file_dialog.FileSelected -= ImportSSL; 
		Globals.file_dialog.Canceled -= Cancel;
		menu.CloseMenu();
    }
}
