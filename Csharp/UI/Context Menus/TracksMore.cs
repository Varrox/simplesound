using System;
using System.IO;
using Godot;

public partial class TracksMore : ContextMenuButton {
    [Export] public TrackDisplay track_display;

    public const int SEP_1 = 2, SEP_2 = 5;

    public const int ADD_TO_PLAYLIST = 0, EDIT_METADATA = 1, REMOVE_TRACK = 3, DELETE_TRACK = 4, SHARE_TRACK = 6;

    public const int COPY_SHARE_LINK = 0, OPEN_TRACK_IN_FILE_EXPLORER = 1;

    public ThemePopupMenu add_to_playlist_popup_menu, share_popup_menu;

    public override void _Ready() {
        base._Ready();

        SetupTrackOptions(GetPopup(), out add_to_playlist_popup_menu, out share_popup_menu);

        add_to_playlist_popup_menu.AboutToPopup += () => SetupAddToPlaylistOptions(add_to_playlist_popup_menu);
        add_to_playlist_popup_menu.IdPressed += _AddToPlaylistItemPressed;
        
        share_popup_menu.IdPressed += _ShareItemPressed;

        MouseEntered += Show;
        GetPopup().PopupHide += Hide;

        Hide();
    }

    public static void SetupAddToPlaylistOptions(PopupMenu popup) {
        popup.Clear();

        for(int i = 0; i < Main.playlists.Count; i++) {
            popup.AddIconItem(Main.playlists[i].cover_texture ?? ApplicationManager.GetThemeIcon("default_cover_icon"), Main.playlists[i].name);
        }
    }

    public static void SetupTrackOptions(PopupMenu popup, out ThemePopupMenu add_to_playlist_popup, out ThemePopupMenu share_popup) {
        // Add To Playlist

        add_to_playlist_popup = new ThemePopupMenu();

        popup.AddSubmenuNodeItem("Add to Playlist", add_to_playlist_popup, ADD_TO_PLAYLIST);
        popup.SetItemIcon(ADD_TO_PLAYLIST, ApplicationManager.GetThemeIcon("plus_icon"));

        SetupAddToPlaylistOptions(add_to_playlist_popup);

        // Normal Options

        popup.AddIconItem(ApplicationManager.GetThemeIcon("pen_icon"), "Edit Metadata", EDIT_METADATA);

        popup.AddSeparator("", SEP_1);

        popup.AddIconItem(ApplicationManager.GetThemeIcon("minus_icon"), "Remove Track from Playlist", REMOVE_TRACK);

        popup.AddIconItem(ApplicationManager.GetThemeIcon("x_icon"), "Delete Track File", DELETE_TRACK);

        popup.AddSeparator("", SEP_2);

        // Share Menu

        share_popup = new ThemePopupMenu();

        popup.AddSubmenuNodeItem("Share", share_popup, SHARE_TRACK);
        popup.SetItemIcon(SHARE_TRACK, ApplicationManager.GetThemeIcon("upload_icon"));

        share_popup.AddIconItem(ApplicationManager.GetThemeIcon("loop_icon"), "Copy Share Link", COPY_SHARE_LINK);

        share_popup.AddIconItem(ApplicationManager.GetThemeIcon("folder_icon"), "Open Track in File Explorer", OPEN_TRACK_IN_FILE_EXPLORER);
    }

    protected override void _ItemPressed(long id) {
        base._ItemPressed(id);

		TrackOptionSelected(id, track_display.playlist_index, track_display.track_index, track_display.track_data, add_to_playlist_popup_menu);
    }

    private void _AddToPlaylistItemPressed(long id) {
        base._ItemPressed(id);

        AddToPlaylistItemSelected(id, track_display.playlist_index, track_display.track_index);
    }

    private void _ShareItemPressed(long id) {
        base._ItemPressed(id);

        ShareItemSelected(id, track_display.playlist_index, track_display.track_index);
    }

    public static void TrackOptionSelected(long id, int playlist_index, int track_index, TrackData track_data, PopupMenu add_to_playlist_popup) {
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
        Main.instance.Pause();

        Globals.attribute_editor.Open(track_data);
        
        void SubmitMeta() {
            if (Globals.attribute_editor.cancelled) {
                Globals.attribute_editor.OnClose -= SubmitMeta;
                return;
            }

            string track_file = Main.playlists[playlist_index].tracks[track_index];
            
            Metadata.SetData(track_file, 
                Globals.attribute_editor.name_field.Text, 
                Globals.attribute_editor.artist_field.Text, 
                Globals.attribute_editor.cover_path, 
                Globals.attribute_editor.share_link_field.Text, 
                Globals.attribute_editor.explicit_lyrics_field.ButtonPressed
            );

            Player.instance.UpdateDisplayedInfo();

            TrackData new_track_data = new TrackData {
                title = Globals.attribute_editor.name_field.Text,
                artist = Globals.attribute_editor.artist_field.Text,
                explicit_lyrics = Globals.attribute_editor.explicit_lyrics_field.ButtonPressed,
                share_link = Globals.attribute_editor.share_link_field.Text,
                time = track_data.time,
                corrupt = Metadata.IsFileCorrupt(track_file), // Recheck just incase
            };

            Main.instance.tracks_visualizer.UpdateTrack(track_index, new_track_data, Tools.GetMediaCover(track_file) ?? ApplicationManager.GetThemeIcon("default_cover_icon"));
            
            Globals.attribute_editor.OnClose -= SubmitMeta;
        }
        
        Globals.attribute_editor.OnClose += SubmitMeta;
    }

    public static void RemoveTrackFromPlaylist(int playlist_index, int track_index) {
        Main.playlists[playlist_index].RemoveTrack(track_index);
        Main.playlists[playlist_index].Save();

        if(Main.playlist_index == playlist_index) {
            if (Main.track_index > track_index) {
                Main.track_index--;
            }
        }

        Main.instance.Refresh();
    }

    public static void DeleteTrackFromPlaylist(int playlist_index, int track_index) {
        Main.playlists[playlist_index].DeleteTrack(track_index);
        Main.playlists[playlist_index].Save();

        if(Main.playlist_index == playlist_index) {
            if (Main.track_index > track_index) {
                Main.track_index--;
            }
        }

        Main.instance.Refresh();
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
        if(!ApplicationManager.save_data.application_settings.remove_track_warning) {
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
                ApplicationManager.save_data.application_settings.remove_track_warning = false;
                ApplicationManager.save_data.Save();
            }

            if (confirm == Confirm.Declined) return;

            RemoveTrackFromPlaylist(playlist_index, track_index);
        }

        confirmation.OnClose += Confirmed;
    }

    public static void AddToPlaylistItemSelected(long id, int playlist_index, int track_index) {
        string track_file = Main.playlists[playlist_index].tracks[track_index];

        Main.playlists[(int)id].AddTrack(track_file);
        Main.playlists[(int)id].Save();

        Main.instance.Refresh();
    }

    public static void ShareItemSelected(long id, int playlist_index, int track_index) {
        string track_file = Main.playlists[playlist_index].tracks[track_index];

		switch(id) {
			case COPY_SHARE_LINK:
                DisplayServer.ClipboardSet(Metadata.GetShareLink(track_file));
				break;
			case OPEN_TRACK_IN_FILE_EXPLORER:
                OS.ShellShowInFileManager(track_file);
				break;
		}
    }
}