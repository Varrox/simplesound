public partial class PlaylistsMore : ContextMenuButton {

    public const int EDIT_PLAYLIST = 0, OPEN_PLAYLIST_FILE = 1, SAVE_PLAYLIST_AS_ZIP = 2, DELETE_PLAYLIST = 3;

    public override void _Ready() {
        base._Ready();

        GetPopup().AddItem("Edit", EDIT_PLAYLIST);
        GetPopup().SetItemDisabled(EDIT_PLAYLIST, true);

        GetPopup().AddItem("Open playlist file", OPEN_PLAYLIST_FILE);
        GetPopup().SetItemDisabled(OPEN_PLAYLIST_FILE, true);

        GetPopup().AddItem("Save as ZIP file", SAVE_PLAYLIST_AS_ZIP);
        GetPopup().SetItemDisabled(SAVE_PLAYLIST_AS_ZIP, true);

        GetPopup().AddItem("Delete playlist", DELETE_PLAYLIST);
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
			case OPEN_PLAYLIST_FILE:
				break;
            case SAVE_PLAYLIST_AS_ZIP:
                break;
		}
    }
}
