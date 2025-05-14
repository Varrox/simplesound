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
		if (Globals.player.interrupt())
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
			string[] files = Directory.GetFiles(SaveSystem.ImportSongs(creator.songs.ToArray(), creator.playlist_name.Text, false));
			Playlist playlist = new Playlist(creator.playlist_name.Text, SaveSystem.ImportCover(creator.cover_path, creator.playlist_name.Text), new List<string>(files));
			
			if (creator.backgroundThemeEnabled.ButtonPressed)
				playlist.customInfo.overlayColor = "#" + creator.backgroundTheme.Color.ToHtml();

			if (creator.album.ButtonPressed)
				playlist.Type = Playlist.PlaylistType.Album;

			if (creator.artist.Text.Trim() != "")
				playlist.Artist = creator.artist.Text;

			Tools.AddToArray(ref Globals.main.playlists, playlist.Save());
			GD.Print(Globals.main.playlists);
			SaveSystem.SaveAllPlaylists(Globals.main.playlists);
            Globals.main.Refresh();
		}

		Globals.player.interrupted = false;
	}
}
