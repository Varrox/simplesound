using System.IO;
using Godot;

public partial class TracksMore : ContextMenuButton {
    [Export] public TrackDisplay track_display;

    public const int EDIT_METADATA = 0, REMOVE_TRACK = 1, DELETE_TRACK = 2, SHARE_TRACK = 3;

    public const int COPY_SHARE_LINK = 0, OPEN_TRACK_IN_FILE_BROWSER = 1;

    public ThemePopupMenu share_popup_menu;

    public override void _Ready() {
        base._Ready();

        GetPopup().AddItem("Edit metadata", EDIT_METADATA);
        GetPopup().SetItemDisabled(EDIT_METADATA, true);

        GetPopup().AddItem("Remove track from playlist", REMOVE_TRACK);
        GetPopup().SetItemDisabled(REMOVE_TRACK, true);

        GetPopup().AddItem("Delete track file", DELETE_TRACK);
        GetPopup().SetItemDisabled(DELETE_TRACK, true);

        share_popup_menu = new ThemePopupMenu();

        GetPopup().AddSubmenuNodeItem("Share", share_popup_menu, SHARE_TRACK);
        GetPopup().SetItemDisabled(SHARE_TRACK, true);

        // Share Menu

        share_popup_menu.AddIconItem(ApplicationManager.GetThemeIcon("loop_icon"), "Copy share link", COPY_SHARE_LINK);
        share_popup_menu.SetItemDisabled(COPY_SHARE_LINK, true);

        share_popup_menu.AddIconItem(ApplicationManager.GetThemeIcon("folder_icon"), "Open track in file browser", OPEN_TRACK_IN_FILE_BROWSER);
        share_popup_menu.SetItemDisabled(OPEN_TRACK_IN_FILE_BROWSER, true);

        MouseEntered += Show;
        GetPopup().PopupHide += Hide;

        Hide();
    }

    protected override void _ItemPressed(long id) {
        base._ItemPressed(id);

		switch(id) {
			case EDIT_METADATA:
				break;
			case REMOVE_TRACK:
                RemoveTrack();
				break;
			case DELETE_TRACK:
                DeleteTrack();
				break;
		}
    }

    public void EditMetadata() {


        if (!Globals.player.Interrupt()) return;

        string track_file = Globals.main.playlists[track_display.playlist_index].tracks[track_display.track_index];;

        Globals.attribute_editor.Open(Metadata.GetName(track_file), Metadata.GetArtist(track_file), Metadata.GetShareLink(track_file), Metadata.IsExplicit(track_file));
        
        void SubmitMeta() {
            if (Globals.attribute_editor.cancelled) {
                Globals.player.interrupted = false;
                Globals.attribute_editor.OnClose -= SubmitMeta;
            }
            
            Metadata.SetData(track_file, 
                Globals.attribute_editor.name_field.Text, 
                Globals.attribute_editor.artist_field.Text, 
                Globals.attribute_editor.cover_path, 
                Globals.attribute_editor.share_link_field.Text, 
                Globals.attribute_editor.explicit_lyrics_field.ButtonPressed
            );

            Globals.player.OnLoadTrack();

            TrackData track_data = new TrackData {
                title = Globals.attribute_editor.name_field.Text,
                artist = Globals.attribute_editor.artist_field.Text,
                time = Tools.SecondsToTimestamp(Metadata.GetTotalTime(track_file)),
                explicit_lyrics = Globals.attribute_editor.explicit_lyrics_field.ButtonPressed,
                corrupt = Metadata.IsFileCorrupt(track_file)
            };

            Texture2D default_cover_icon = ApplicationManager.GetThemeIcon("default_cover_icon");
            Globals.main.tracks_visualizer.UpdateTrack(track_display.track_index, track_data, Tools.GetMediaCover(track_file) ?? default_cover_icon);

            Globals.player.interrupted = false;
            Globals.attribute_editor.OnClose -= SubmitMeta;
        }
        
        Globals.attribute_editor.OnClose += SubmitMeta;
    }

    public void RemoveTrackFromPlaylist() {
        Globals.main.playlists[track_display.playlist_index].tracks.RemoveAt(track_display.track_index);
        Globals.main.playlists[track_display.playlist_index].Save();

        Globals.main.Refresh();
    }

    public void DeleteTrack() {
        ConfirmationWindow confirmation = (ConfirmationWindow)Globals.confirmation_window.Instantiate();

        confirmation.message = $"Do you really want to delete track file '{track_display.track_name_label.GetParsedText()}'?";
        confirmation.accept_text = "Yes";
        confirmation.decline_text = "No";
        confirmation.cancel_text = "";

        confirmation.front = true;
        confirmation.free_on_close = true;
        
        GetTree().Root.AddChild(confirmation);

        void Confirmed(Confirm confirm) {
            if (confirm == Confirm.Declined) return;

            string track_file = Globals.main.playlists[track_display.playlist_index].tracks[track_display.track_index];

            File.Delete(track_file);
            RemoveTrackFromPlaylist();
        }

        confirmation.OnClose += Confirmed;
    }

    public void RemoveTrack() {
        if(!Globals.save_data.application_settings.remove_track_warning) {
            RemoveTrackFromPlaylist();
            return;
        }

        ConfirmationWindow confirmation = (ConfirmationWindow)Globals.confirmation_window.Instantiate();

        confirmation.message = $"Do you really want to remove track '{track_display.track_name_label.GetParsedText()}'?\n(This will not delete its file.)";
        confirmation.accept_text = "Yes";
        confirmation.decline_text = "No";
        confirmation.cancel_text = "";

        confirmation.front = true;
        confirmation.show_ignore = true;
        confirmation.free_on_close = true;
        
        GetTree().Root.AddChild(confirmation);

        void Confirmed(Confirm confirm) {
            if (confirmation.ignored) {
                Globals.save_data.application_settings.remove_track_warning = false;
                Globals.save_data.Save();
            }

            if (confirm == Confirm.Declined) return;

            RemoveTrackFromPlaylist();
        }

        confirmation.OnClose += Confirmed;
    }

    private void _ShareItemPressed(long id) {
        base._ItemPressed(id);

        string track_file = Globals.main.playlists[track_display.playlist_index].tracks[track_display.track_index];

		switch(id) {
			case COPY_SHARE_LINK:
                DisplayServer.ClipboardSet(Metadata.GetShareLink(track_file));
				break;
			case OPEN_TRACK_IN_FILE_BROWSER:
                OS.ShellShowInFileManager(track_file);
				break;
		}
    }
}