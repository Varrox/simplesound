using Godot;
using SSLParser;
using System.IO;

public partial class ImportPlaylist : Button
{
	[Export] ContextMenu menu;

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ButtonDown += ImportFile;
	}

	public void ImportFile()
	{
		Globals.SetFileDialogPlaylist();
		Globals.file_dialog.Popup();

        Globals.file_dialog.FileSelected += ImportSSL;
        Globals.file_dialog.Canceled += Cancel;
    }

	public void ImportSSL(string ssl_file)
	{
		string newPath = Path.Combine(SaveSystem.UserData, "Playlists", Path.GetFileName(ssl_file));

		File.Copy(ssl_file, newPath);

		Playlist playlist = MainParser.ParsePlaylist(File.ReadAllText(newPath));

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
