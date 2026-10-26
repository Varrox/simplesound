using System.Collections.Generic;
using Godot;

public partial class PlaylistsMore : ContextMenuButton {
    [Export] public PlaylistDisplay playlist_display;

    public const int SEP_1 = 1, SEP_2 = 4, SEP_3 = 7;

    public const int EDIT_PLAYLIST = 0, OPEN_PLAYLIST_IN_FILE_BROWER = 2, OPEN_PLAYLIST_FILE = 3, SAVE_PLAYLIST_AS_ZIP = 5, ADD_TO_OTHER_PLAYLIST = 6, DELETE_PLAYLIST = 8;

    public ThemePopupMenu add_to_other_playlist_popup_menu;

    public override void _Ready() {
        base._Ready();

        // Edit

        GetPopup().AddIconItem(ApplicationManager.GetThemeIcon("pen_icon"), "Edit", EDIT_PLAYLIST);
        GetPopup().SetItemDisabled(EDIT_PLAYLIST, true);

        GetPopup().AddSeparator("", SEP_1);

        // Open

        GetPopup().AddIconItem(ApplicationManager.GetThemeIcon("folder_icon"), "Open playlist in file browser", OPEN_PLAYLIST_IN_FILE_BROWER);

        GetPopup().AddIconItem(ApplicationManager.GetThemeIcon("file_icon"), "Open playlist file", OPEN_PLAYLIST_FILE);

        GetPopup().AddSeparator("", SEP_2);

        // Save

        GetPopup().AddIconItem(ApplicationManager.GetThemeIcon("zip_icon"), "Save as ZIP file", SAVE_PLAYLIST_AS_ZIP);
        GetPopup().SetItemDisabled(SAVE_PLAYLIST_AS_ZIP, true);

        add_to_other_playlist_popup_menu = new ThemePopupMenu();

        GetPopup().AddSubmenuNodeItem("Add to other playlist", add_to_other_playlist_popup_menu, ADD_TO_OTHER_PLAYLIST);
        GetPopup().SetItemIcon(ADD_TO_OTHER_PLAYLIST, ApplicationManager.GetThemeIcon("plus_icon"));

        UpdateAddToOtherPlaylistOptions();

        add_to_other_playlist_popup_menu.AboutToPopup += UpdateAddToOtherPlaylistOptions;
        add_to_other_playlist_popup_menu.IdPressed += _UpdateAddToOtherPlaylistItemPressed;

        GetPopup().AddSeparator("", SEP_3);

        // Delete

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

    private void _UpdateAddToOtherPlaylistItemPressed(long id) {
        base._ItemPressed(id);

        UpdateAddToOtherPlaylistItemSelected(id, playlist_display.playlist_index);
    }

    public static void UpdateAddToOtherPlaylistItemSelected(long id, int playlist_index) {
        for(int i = 0; i < Globals.main.playlists[playlist_index].tracks.Count; i++) {
            Globals.main.playlists[(int)id].AddTrack(Globals.main.playlists[playlist_index].tracks[i]);
        }

        Globals.main.playlists[(int)id].Save();

        Globals.main.Refresh();
    }

    private void UpdateAddToOtherPlaylistOptions() {
        add_to_other_playlist_popup_menu.Clear();

        for(int i = 0; i < Globals.main.playlists.Count; i++) {
            add_to_other_playlist_popup_menu.AddIconItem(Globals.main.playlists[i].cover_texture ?? ApplicationManager.GetThemeIcon("default_cover_icon"), Globals.main.playlists[i].name);
        }
    }
}
