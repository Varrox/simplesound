using Godot;

public partial class ShareContextMenuOpener : ContextMenuOpener
{
	[Export] public Button share_link, file_browse;

    public static string file, link;

	public override void _Ready()
	{
		base._Ready();
		OnOpen += GetTrackData;
		OnClose += () => Icon = ApplicationManager.theme.GetIcon("meatballs_icon", Constants.THEME_TYPE);

		share_link.ButtonUp += CopyShareLinkToClipboard;
		file_browse.ButtonUp += FileBrowseTrack;
    }

    public void CopyShareLinkToClipboard()
    {
        DisplayServer.ClipboardSet(link);
    }

	public void FileBrowseTrack()
	{
        OS.ShellShowInFileManager(file);
	}

    public void GetTrackData()
	{
        file = Globals.main.playlists[Globals.main.looked_at_playlist].tracks[TracksContextMenuOpener.selected_track];
		link = Metadata.GetShareLink(file);

		Icon = ApplicationManager.theme.GetIcon("back_arrow_icon", Constants.THEME_TYPE);;
	}
}
