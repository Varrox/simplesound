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

            string file_name = creator.playlist_name.Text.Replace("\\", "-").Replace("/", "-");

            var cover_path = sync ? SaveSystem.ImportCover(creator.cover_path, file_name) : creator.cover_path;

            List<string> songs = sync ? SaveSystem.ImportSongs(creator.songs.ToArray(), file_name, false) : creator.songs;

			Playlist playlist = new Playlist(creator.playlist_name.Text, cover_path, songs);
			
			playlist.PathName = file_name;

			if (creator.backgroundThemeEnabled.ButtonPressed)
				playlist.customInfo.overlay_color = "#" + creator.backgroundTheme.Color.ToHtml();

			if (creator.album.ButtonPressed)
				playlist.Type = Playlist.PlaylistType.Album;

			if (creator.artist.Text.Trim() != "")
				playlist.Artist = creator.artist.Text;

			creator.Clear();

			for(int i = 0; i < creator.songs.Count; i++)
			{
				GD.Print($"Song {i}: {creator.songs[i]}");
			}

			Globals.main.playlist_paths.Add(playlist.Save());

			SaveSystem.SaveAllPlaylists(Globals.main.playlist_paths.ToArray());
            Globals.main.Refresh();
		}

		Globals.player.interrupted = false;
	}
}
