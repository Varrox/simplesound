using Godot;
using System;

public partial class ShareMenu : ContextMenuOpener
{
	[Export] public Button share_link, file_browse;

	[Export] public Texture2D share, back;

    public static string file, link;

	public override void _Ready()
	{
		base._Ready();
		OnOpen += GetSongData;
		OnClose += () => Icon = share;

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
        file = Globals.main.playlists[Globals.main.looked_at_playlist].songs[SongsMore.song];
		link = Metadata.GetShareLink(file);

		Icon = back;
	}
}
