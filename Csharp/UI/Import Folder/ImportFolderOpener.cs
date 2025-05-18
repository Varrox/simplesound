using Godot;
using System.Collections.Generic;
using System.IO;

public partial class ImportFolderOpener : EditorWindowOpener
{
    [Export] ContextMenu menu;
    public override void _Ready()
    {
        base._Ready();

        ButtonDown += OpenImporter;
        window.OnClose += CreatePlaylist;
    }

    public void OpenImporter()
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
            List<string> files = SaveSystem.ImportSongs(creator.songs.ToArray(), creator.playlist_name.Text, false);
            Playlist playlist = new Playlist(creator.playlist_name.Text, SaveSystem.ImportCover(creator.cover_path, creator.playlist_name.Text), new List<string>(files));

            if (creator.backgroundThemeEnabled.ButtonPressed)
                playlist.customInfo.overlayColor = "#" + creator.backgroundTheme.Color.ToHtml();

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
