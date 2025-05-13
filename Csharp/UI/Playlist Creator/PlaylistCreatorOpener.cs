using Godot;
using System.IO;
using System.Collections.Generic;

public partial class PlaylistCreatorOpener : EditorWindowOpener
{
	public override void _Ready()
	{
		base._Ready();

		ButtonDown += OpenCreator;
		window.OnClose += SubmitMeta;
	}

	public void OpenCreator()
	{
		if (Globals.player.interrupt())
		{
			(window as PlaylistCreator).Open();
		}
	}

	public void SubmitMeta()
	{
		PlaylistCreator creator = window as PlaylistCreator;

		if (!creator.cancelled)
		{
			string[] files = Directory.GetFiles(SaveSystem.ImportSongs(creator.songs.ToArray(), creator.playlist_name.Text, false));
			Playlist playlist = new Playlist(creator.playlist_name.Text, creator.cover_path, new List<string>(files));
			
			if (creator.backgroundThemeEnabled.ButtonPressed)
				playlist.customInfo.overlayColor = creator.backgroundTheme.Color.ToHtml();

			if (creator.album.ButtonPressed)
				playlist.Type = Playlist.PlaylistType.Album;

			if (creator.artist.Text.Trim() != "")
				playlist.Artist = creator.artist.Text;

			Tools.AddToArray(ref Globals.main.playlists, playlist.Save());
			SaveSystem.SaveAllPlaylists(Globals.main.playlists);
            Globals.main.Refresh();
		}

		Globals.player.interrupted = false;
	}
}
