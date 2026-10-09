using Godot;
using System.Collections.Generic;

public partial class PlaylistCreatorOpener : EditorWindowOpener
{
	[Export] public ContextMenu menu;
	public override void _Ready()
	{
		ButtonUp += OpenCreator;
		window.OnClose += CreatePlaylist;
	}

	public void OpenCreator()
	{
		if (Globals.player.Interrupt())
		{
			menu.opener.CloseMenu();
			(window as PlaylistCreator).Open();
		}
	}

	public void CreatePlaylist()
	{
		PlaylistCreator creator = window as PlaylistCreator;

		if (!creator.cancelled)
		{
			bool sync = creator.cloud_sync_enabled_field.ButtonPressed;

            string file_name = creator.playlist_name_field.Text.Replace('\\', '-').Replace('/', '-').Replace(':', '-');

            var cover_path = sync ? SaveSystem.ImportCover(creator.cover_path, file_name) : creator.cover_path;

            List<string> tracks = sync ? SaveSystem.ImportTracks(creator.tracks.ToArray(), file_name, false) : new(creator.tracks);

			Playlist playlist = new Playlist(creator.playlist_name_field.Text, cover_path, tracks);

			if (creator.background_color_enabled_field.ButtonPressed)
				playlist.custom_info.overlay_color = "#" + creator.background_color_field.Color.ToHtml();

			if (creator.album_field.ButtonPressed)
				playlist.type = Playlist.PlaylistType.Album;

			if (creator.artist_field.Text.Trim() != "")
				playlist.artist = creator.artist_field.Text;

            creator.Clear();

			Globals.main.playlist_paths.Add(playlist.Save());
			Globals.main.SetSaveData();
            Globals.main.Refresh();
        }

		Globals.player.interrupted = false;
	}
}
