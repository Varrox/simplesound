using Godot;
using System.IO;
using System.Collections.Generic;

public partial class PlaylistCreatorOpener : EditorWindowOpener
{
	[Export] ContextMenu menu;
	public override void _Ready()
	{
		base._Ready();

		ButtonDown += OpenCreator;
		window.OnClose += CreatePlaylist;
	}

	public void OpenCreator()
	{
		if (Globals.player.Interrupt())
		{
			menu.CloseMenu();
			(window as PlaylistCreator).Open();
		}
	}

	public void CreatePlaylist()
	{
		PlaylistCreator creator = window as PlaylistCreator;

		if (!creator.cancelled)
		{
			bool sync = creator.cloudSync.ButtonPressed;
			string filename = creator.playlist_name.Text.Replace("\\", "-").Replace("/", "-");
            List<string> files = sync ? SaveSystem.ImportSongs(creator.songs.ToArray(), filename, false) : creator.songs;
			Playlist playlist = new Playlist(creator.playlist_name.Text, sync ? SaveSystem.ImportCover(creator.cover_path, filename) : creator.cover_path, files);
			playlist.PathName = filename;

			if (creator.backgroundThemeEnabled.ButtonPressed)
				playlist.customInfo.overlayColor = "#" + creator.backgroundTheme.Color.ToHtml();

			if (creator.album.ButtonPressed)
				playlist.Type = Playlist.PlaylistType.Album;

			if (creator.artist.Text.Trim() != "")
				playlist.Artist = creator.artist.Text;

			creator.Clear();

			Tools.AddToArray(ref Globals.main.playlists, playlist.Save());
			SaveSystem.SaveAllPlaylists(Globals.main.playlists);
            Globals.main.Refresh();
		}

		Globals.player.interrupted = false;
	}
}
