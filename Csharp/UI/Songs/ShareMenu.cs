using Godot;
using SSLParser;
using System;

public partial class ShareMenu : ContextMenu
{
	[Export] public Button share_link, file_browse;

	[Export] public Texture2D Share, Back;

	public static int song;
    public static string file, link;
	public static ShareMenu instance;

	public override void _Ready()
	{
		base._Ready();
		instance = this;
		OnOpen += GetSongData;
		OnClose += () => Icon = Share;

		share_link.ButtonUp += CopyShareLinkToClipboard;
		file_browse.ButtonUp += FileBrowseSong;
    }

    public void CopyShareLinkToClipboard()
    {
        DisplayServer.ClipboardSet(link);
    }

	public void FileBrowseSong()
	{
        OS.ShellShowInFileManager(file);
	}

    public void GetSongData()
	{
		Playlist playlist;

		if (Globals.main.playlist_index == Globals.main.current_looked_at_playlist)
		{
			playlist = Globals.main.playlist;
		}
		else
		{
            playlist = MainParser.ParsePlaylist(Globals.main.playlists[Globals.main.current_looked_at_playlist]);
        }

        file = playlist.Songs[song];
		link = Metadata.GetShareLink(file);

		Icon = Back;
	}
}
