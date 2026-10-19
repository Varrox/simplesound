using Godot;
using System.Collections.Generic;

public partial class TracksVisualizer : CarouselScrollContainer {
    [Export] public TextureRect cover;
    
    [Export] public Label playlist_name_label;
    [Export] public Label track_count_label;

    private int current_playlist;

    private Playlist GetPlaylist() {
        if (Globals.main.playlists != null)
            if (current_playlist < Globals.main.playlists.Count)
                return Globals.main.playlists[current_playlist];
        return null;
    }

    public override void _Ready() {
        base._Ready();

        Globals.main.playlist_visualizer.OnSelectPlaylist += LoadPlaylist;
    }

    private Texture2D GetCover(in Playlist playlist, in int track) {
        Texture2D default_cover_icon = ApplicationManager.theme.GetIcon("default_cover_icon", Constants.THEME_TYPE);
        return (playlist.type == Playlist.PlaylistType.Album) ? null : (ConvertToGodot.GetMediaCover(playlist.tracks[track]) ?? default_cover_icon);
    }

    protected override void _UpdateItem(int item_index, Control item) {
        Playlist playlist = GetPlaylist();

        TrackData data = GetTrackData(item_index, playlist);
        (item as TrackDisplay).Init(item_index, current_playlist, data, playlist.type, GetCover(playlist, item_index));
    }

    private TrackData GetTrackData(int track, in Playlist playlist) {
        return new TrackData(playlist.tracks[track]);
    }

    private void LoadPlaylist(int playlist_index) {
        current_playlist = playlist_index;

        ScrollVertical = 0;

        Update();
    }

    public void UpdateTrack(int index, in TrackData track_data, in Texture2D cover) {
        int first_visible_track = GetFirstVisibleItemIndex();

        if (Globals.main.looked_at_playlist != Globals.main.playlist_index || index < first_visible_track || index >= (items.Count + first_visible_track))
            return;
        
        (items[index - first_visible_track] as TrackDisplay).Init(index, current_playlist, track_data, GetPlaylist().type, cover);
    }

    public void Update() {
        Playlist playlist = GetPlaylist();
        
        cover.Texture = playlist.cover_texture ?? ApplicationManager.theme.GetIcon("default_cover_icon", Constants.THEME_TYPE);
        playlist_name_label.Text = playlist.name;

        if (playlist.tracks != null) track_count_label.Text = $"{playlist.tracks.Count} track" + (playlist.tracks.Count != 1 ? "s" : "");
        else track_count_label.Text = "0 tracks";

        total_items = playlist.tracks.Count;

        Refresh();
    }
}
