using Godot;
using Godot.Collections;

public partial class PlaylistsVisualizer : ScrollContainer {
	[Export] public PackedScene template;
    [Export] public VBoxContainer container;

    public Array<PlaylistDisplay> playlist_displays = new Array<PlaylistDisplay>();

    [Signal]
    public delegate void OnSelectPlaylistEventHandler(int playlist_index);

    public void LoadAllPlaylistVisuals() { // only done once for init
        for (int i = 0; i < Main.playlist_paths.Count; i++) LoadPlaylist(i);
    }

    public void LoadPlaylist(int i) {
        PlaylistDisplay playlist_display = template.Instantiate() as PlaylistDisplay;
        container.AddChild(playlist_display);
        playlist_displays.Add(playlist_display);

        playlist_display.Init(Main.playlists[i], i);
    }

    public void UpdatePlaylists() {
        int end = playlist_displays.Count;

        if (playlist_displays.Count < Main.playlists.Count) { // less
            for(int i = playlist_displays.Count; i < Main.playlists.Count; i++) LoadPlaylist(i);
        }
        else if (playlist_displays.Count > Main.playlists.Count) { // more
            for (int i = playlist_displays.Count; i < Main.playlists.Count; i++) { // delete the extra
                playlist_displays[i].QueueFree();
                playlist_displays.RemoveAt(i);
            }

            end = Main.playlists.Count;
        }

        for(int i = 0; i < end; i++) {
            playlist_displays[i].Init(Main.playlists[i], i);
        }
    }
}
