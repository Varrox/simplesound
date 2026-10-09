using Godot;
using System.Collections.Generic;

public partial class ImportFolderOpener : EditorWindowOpener
{
    [Export] public ContextMenu menu;
    public override void _Ready()
    {
        ButtonUp += OpenImporter;
        window.OnClose += CreatePlaylist;
    }

    public void OpenImporter()
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
            List<string> files = SaveSystem.ImportTracks(creator.tracks.ToArray(), creator.playlist_name_field.Text, false);
            Playlist playlist = new Playlist(creator.playlist_name_field.Text, SaveSystem.ImportCover(creator.cover_path, creator.playlist_name_field.Text), new List<string>(files));

            if (creator.background_color_enabled_field.ButtonPressed)
                playlist.custom_info.overlay_color = "#" + creator.background_color_field.Color.ToHtml();

            if (creator.album_field.ButtonPressed)
                playlist.type = Playlist.PlaylistType.Album;

            if (creator.artist_field.Text.Trim() != "")
                playlist.artist = creator.artist_field.Text;

            Globals.main.playlist_paths.Add(playlist.Save());
            Globals.main.SetSaveData();
            Globals.main.Refresh();
        }

        Globals.player.interrupted = false;
    }
}
