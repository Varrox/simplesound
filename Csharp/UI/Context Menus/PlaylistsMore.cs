using Godot;

public partial class PlaylistsMore : ContextMenuButton {
    [Export] public PlaylistDisplay playlist_display;

    public const int SEP_1 = 3, SEP_2 = 5;

    public const int EDIT_PLAYLIST = 0, OPEN_PLAYLIST_IN_FILE_BROWER = 1, OPEN_PLAYLIST_FILE = 2, SAVE_PLAYLIST_AS_ZIP = 4, DELETE_PLAYLIST = 6;

    public override void _Ready() {
        base._Ready();

        GetPopup().AddItem("Edit", EDIT_PLAYLIST);
        GetPopup().SetItemDisabled(EDIT_PLAYLIST, true);

        GetPopup().AddIconItem(ApplicationManager.GetThemeIcon("folder_icon"), "Open playlist in file browser", OPEN_PLAYLIST_IN_FILE_BROWER);

        GetPopup().AddIconItem(ApplicationManager.GetThemeIcon("file_icon"), "Open playlist file", OPEN_PLAYLIST_FILE);

        GetPopup().AddSeparator("", SEP_1);

        GetPopup().AddIconItem(ApplicationManager.GetThemeIcon("zip_icon"), "Save as ZIP file", SAVE_PLAYLIST_AS_ZIP);
        GetPopup().SetItemDisabled(SAVE_PLAYLIST_AS_ZIP, true);

        GetPopup().AddSeparator("", SEP_2);

        GetPopup().AddIconItem(ApplicationManager.GetThemeIcon("x_icon"), "Delete playlist", DELETE_PLAYLIST);
        GetPopup().SetItemDisabled(DELETE_PLAYLIST, true);

        MouseEntered += Show;
        GetPopup().PopupHide += Hide;

        Hide();
    }

    protected override void _ItemPressed(long id) {
        base._ItemPressed(id);

		switch(id) {
			case EDIT_PLAYLIST:
				break;
			case DELETE_PLAYLIST:
				break;
            case OPEN_PLAYLIST_IN_FILE_BROWER:
                OS.ShellShowInFileManager(Globals.main.playlists[playlist_display.playlist_index].path);
				break;
			case OPEN_PLAYLIST_FILE:
                OS.ShellOpen(Globals.main.playlists[playlist_display.playlist_index].path);
				break;
            case SAVE_PLAYLIST_AS_ZIP:
                break;
		}
    }
}
