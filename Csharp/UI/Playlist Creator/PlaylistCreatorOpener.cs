using Godot;
using System.Collections.Generic;

public partial class PlaylistCreatorOpener : EditorWindowOpener
{
	[Export] ContextMenu menu;
	public override void _Ready()
	{
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

            string file_name = creator.playlist_name.Text.Replace('\\', '-').Replace('/', '-').Replace(':', '-');

            var cover_path = sync ? SaveSystem.ImportCover(creator.cover_path, file_name) : creator.cover_path;

            List<string> songs = sync ? SaveSystem.ImportSongs(creator.songs.ToArray(), file_name, false) : new(creator.songs);

			Playlist playlist = new Playlist(creator.playlist_name.Text, cover_path, songs);

			if (creator.backgroundThemeEnabled.ButtonPressed)
				playlist.custom_info.overlay_color = "#" + creator.backgroundTheme.Color.ToHtml();

			if (creator.album.ButtonPressed)
				playlist.type = Playlist.PlaylistType.Album;

			if (creator.artist.Text.Trim() != "")
				playlist.artist = creator.artist.Text;

            creator.Clear();

			Globals.main.playlist_paths.Add(playlist.Save());
			Globals.main.Save();
            Globals.main.Refresh();
        }

		Globals.player.interrupted = false;
	}
}
