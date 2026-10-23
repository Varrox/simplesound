using System;
using System.IO;
using Godot;

public partial class TracksMore : ContextMenuButton {
    [Export] public TrackDisplay track_display;

    public const int EDIT_METADATA = 0, REMOVE_TRACK = 1, DELETE_TRACK = 2, SHARE_TRACK = 3;

    public const int COPY_SHARE_LINK = 0, OPEN_TRACK_IN_FILE_BROWSER = 1;

    public ThemePopupMenu share_popup_menu;

    public override void _Ready() {
        base._Ready();

        SetupTrackOptions(GetPopup(), out share_popup_menu);

        share_popup_menu.IdPressed += _ShareItemPressed;

        MouseEntered += Show;
        GetPopup().PopupHide += Hide;

        Hide();
    }

    public static void SetupTrackOptions(PopupMenu popup, out ThemePopupMenu share_popup) {
        popup.AddItem("Edit metadata", EDIT_METADATA);

        popup.AddItem("Remove track from playlist", REMOVE_TRACK);

        popup.AddItem("Delete track file", DELETE_TRACK);

        // Share Menu

        share_popup = new ThemePopupMenu();

        popup.AddSubmenuNodeItem("Share", share_popup, SHARE_TRACK);

        share_popup.AddIconItem(ApplicationManager.GetThemeIcon("loop_icon"), "Copy share link", COPY_SHARE_LINK);

        share_popup.AddIconItem(ApplicationManager.GetThemeIcon("folder_icon"), "Open track in file browser", OPEN_TRACK_IN_FILE_BROWSER);
    }

    protected override void _ItemPressed(long id) {
        base._ItemPressed(id);

		TrackOptionSelected(id, track_display.playlist_index, track_display.track_index, track_display.track_data);
    }

    private void _ShareItemPressed(long id) {
        base._ItemPressed(id);

        ShareItemSelected(id, track_display.playlist_index, track_display.track_index);
    }

    public static void TrackOptionSelected(long id, int playlist_index, int track_index, TrackData track_data) {
        switch(id) {
			case EDIT_METADATA:
                EditMetadata(playlist_index, track_index, track_data);
				break;
			case REMOVE_TRACK:
                RemoveTrack(playlist_index, track_index, track_data.title);
				break;
			case DELETE_TRACK:
                DeleteTrack(playlist_index, track_index, track_data.title);
				break;
		}
    }

    public static void EditMetadata(int playlist_index, int track_index, TrackData track_data) {
        if (!Globals.player.Interrupt()) return;

        Globals.attribute_editor.Open(track_data);
        
        void SubmitMeta() {
            if (Globals.attribute_editor.cancelled) {
                Globals.player.interrupted = false;
                Globals.attribute_editor.OnClose -= SubmitMeta;
                return;
            }

            string track_file = Globals.main.playlists[playlist_index].tracks[track_index];
            
            Metadata.SetData(track_file, 
                Globals.attribute_editor.name_field.Text, 
                Globals.attribute_editor.artist_field.Text, 
                Globals.attribute_editor.cover_path, 
                Globals.attribute_editor.share_link_field.Text, 
                Globals.attribute_editor.explicit_lyrics_field.ButtonPressed
            );

            Globals.player.UpdateDisplayedInfo();

            TrackData new_track_data = new TrackData {
                title = Globals.attribute_editor.name_field.Text,
                artist = Globals.attribute_editor.artist_field.Text,
                explicit_lyrics = Globals.attribute_editor.explicit_lyrics_field.ButtonPressed,
                share_link = Globals.attribute_editor.share_link_field.Text,
                time = track_data.time,
                corrupt = Metadata.IsFileCorrupt(track_file), // Recheck just incase
            };

            Globals.main.tracks_visualizer.UpdateTrack(track_index, new_track_data, Tools.GetMediaCover(track_file) ?? ApplicationManager.GetThemeIcon("default_cover_icon"));
            Globals.player.interrupted = false;
            Globals.attribute_editor.OnClose -= SubmitMeta;
        }
        
        Globals.attribute_editor.OnClose += SubmitMeta;
    }

    public static void RemoveTrackFromPlaylist(int playlist_index, int track_index) {
        Globals.main.playlists[playlist_index].RemoveTrack(track_index);
        Globals.main.playlists[playlist_index].Save();

        if(Globals.main.playlist_index == playlist_index) {
            if (Globals.main.track_index > track_index) {
                Globals.main.track_index--;
            }
        }

        Globals.main.Refresh();
    }

    public static void DeleteTrackFromPlaylist(int playlist_index, int track_index) {
        Globals.main.playlists[playlist_index].DeleteTrack(track_index);
        Globals.main.playlists[playlist_index].Save();

        if(Globals.main.playlist_index == playlist_index) {
            if (Globals.main.track_index > track_index) {
                Globals.main.track_index--;
            }
        }

        Globals.main.Refresh();
    }

    public static void DeleteTrack(int playlist_index, int track_index, string track_name) {
        ConfirmationWindow confirmation = (ConfirmationWindow)Globals.confirmation_window.Instantiate();

        confirmation.message = $"Do you really want to delete track file '{track_name}'?";
        confirmation.accept_text = "Yes";
        confirmation.decline_text = "No";
        confirmation.cancel_text = "";

        confirmation.front = true;
        confirmation.free_on_close = true;
        
        ApplicationManager.instance.Root.AddChild(confirmation);

        void Confirmed(Confirm confirm) {
            if (confirm == Confirm.Declined) return;

            DeleteTrackFromPlaylist(playlist_index, track_index);
        }

        confirmation.OnClose += Confirmed;
    }

    public static void RemoveTrack(int playlist_index, int track_index, string track_name) {
        if(!Globals.save_data.application_settings.remove_track_warning) {
            RemoveTrackFromPlaylist(playlist_index, track_index);
            return;
        }

        ConfirmationWindow confirmation = (ConfirmationWindow)Globals.confirmation_window.Instantiate();

        confirmation.message = $"Do you really want to remove track '{track_name}'?\n(This will not delete its file.)";
        confirmation.accept_text = "Yes";
        confirmation.decline_text = "No";
        confirmation.cancel_text = "";

        confirmation.front = true;
        confirmation.show_ignore = true;
        confirmation.free_on_close = true;
        
        ApplicationManager.instance.Root.AddChild(confirmation);

        void Confirmed(Confirm confirm) {
            if (confirmation.ignored) {
                Globals.save_data.application_settings.remove_track_warning = false;
                Globals.save_data.Save();
            }

            if (confirm == Confirm.Declined) return;

            RemoveTrackFromPlaylist(playlist_index, track_index);
        }

        confirmation.OnClose += Confirmed;
    }

    public static void ShareItemSelected(long id, int playlist_index, int track_index) {
        string track_file = Globals.main.playlists[playlist_index].tracks[track_index];

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