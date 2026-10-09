using Godot;
using System.Collections.Generic;

public partial class TracksVisualizer : ScrollContainer
{
    [Export] public PackedScene template;
    [Export] public Control top_card;
    [Export] public VBoxContainer container; // Only children will be track displays. Is a child VBoxContainer of the one containing top and bottom spacers, and top card.
    [Export] public Control top_spacer, bottom_spacer;
    [Export] public TextureRect cover;
    
    [Export] public Label playlist_name_label;
    [Export] public Label track_count_label;

    int last_scroll = -1, last_first_track = -1;
    Vector2 last_size = Vector2.Zero;

    List<TrackDisplay> track_displays;
    Dictionary<int, TrackData> track_datas;

    const int SINGLE_BUTTON_SIZE = 60;
    int top_card_size_y;
    
    int GetTotalHeight() {
        return SINGLE_BUTTON_SIZE * GetPlaylist().songs.Count;
    }

    private Playlist GetPlaylist() {
        return Globals.main.playlists[Globals.main.looked_at_playlist];
    }

    public override void _Ready() {
        top_card_size_y = (int)top_card.Size.Y;

        Globals.main.playlist_visualizer.OnSelectPlaylist += LoadPlaylist;

        track_datas = new Dictionary<int, TrackData>();
        track_displays = new List<TrackDisplay>();
    }

    public override void _Process(double delta) {
        Playlist playlist = GetPlaylist();

        if (playlist == null)
            return;
        
        if (last_scroll != ScrollVertical || last_size != Size) {
            UpdateTracks(playlist);
        }
    }

    private Texture2D GetCover(in Playlist playlist, in int track) {
        return (playlist.type == Playlist.PlaylistType.Album) ? null : ConvertToGodot.GetMediaCover(playlist.songs[track]);
    }

    private void UpdateTracks(in Playlist playlist) {
        int max_displays = (int)Mathf.Ceil(Size.Y / SINGLE_BUTTON_SIZE) + 3;
        int target_displays = Mathf.Min(max_displays, playlist.songs.Count);

        (int top, int bottom) = CalculateSize();

        int first_visible_track = GetFirstVisibleTrack();

        top_spacer.CustomMinimumSize = Vector2.Down * top;
        bottom_spacer.CustomMinimumSize = Vector2.Down * bottom;
        
        bottom_spacer.Visible = target_displays == max_displays;
        top_spacer.Visible = target_displays == max_displays;

        // Check if there is enough displays, if not, add or remove them. when adding a new one, update it.

        if(track_displays.Count != target_displays) // NOTE: Is only true if there is more tracks added (only when previous count was below max_displays), or the size changed.
        {
            if(track_displays.Count > target_displays) // Delete old
            {
                for(int i = track_displays.Count; i >= target_displays; i--)
                {
                    track_displays[i].QueueFree();
                    track_displays.RemoveAt(i);
                }
            }
            else // Add new
            {
                for(int i = track_displays.Count; i < target_displays; i++)
                {
                    track_displays.Add(template.Instantiate() as TrackDisplay);
                    container.AddChild(track_displays[i]);

                    int track = first_visible_track + i;

                    TrackData data = GetTrackData(track, playlist);
                    track_displays[i].Init(track, data, playlist.type, GetCover(playlist, track));
                }
            }
        }

        // If first track has changed, go through and update only the ones that need new covers and data, and shift back old displays.

        int shift = first_visible_track - last_first_track; // The amount to shift by
        int a_shift = Mathf.Abs(shift);

        if(shift != 0 && ScrollVertical > top_card_size_y + SINGLE_BUTTON_SIZE || a_shift < track_displays.Count) // NOTE: Is only true if there has been any scrolling, and past the beginning.
        {
            // If shift is less than 0, then the user has scrolled up.
            // If shift is more than 0, then the user has scrolled down.

            // Scrolling up, requires the last track display to be placed at the beginning, and updated.
            // Scrolling down, requires the first track display to be placed at the end, and updated.

            // Do these ^^^ (|shift|) times

            int last_index = track_displays.Count - 1;

            for(int i = 0; i < a_shift; i++)
            {
                TrackDisplay display;
                int index;

                if(shift < 0)
                {
                    display = track_displays[last_index];
                    track_displays.RemoveAt(last_index);
                    track_displays.Insert(0, display);

                    container.MoveChild(display, 0);

                    index = (a_shift - i) - 1;
                }
                else
                {
                    display = track_displays[0];
                    track_displays.RemoveAt(0);
                    track_displays.Add(display); // Add instead of insert because it is probably faster and makes more sense logic wise.

                    container.MoveChild(display, last_index);

                    index = track_displays.Count - (a_shift - i);
                }

                int track = first_visible_track + index;

                TrackData data = GetTrackData(track, playlist);
                track_displays[index].Init(track, data, playlist.type, GetCover(playlist, track));
            }
        }
        
        last_scroll = ScrollVertical;
        last_size = Size;
        last_first_track = first_visible_track;
    }

    private TrackData GetTrackData(int track, in Playlist playlist)
    {
        if (!track_datas.ContainsKey(track)) {
            if(track < playlist.songs.Count) {
                TrackData data = new TrackData(playlist.songs[track]);
                track_datas[track] = data;
            }
            else track_datas[track] = new TrackData{};
        }
        return track_datas[track];
    }

    private void LoadPlaylist(int playlist_index, Texture2D playlist_cover)
    {
        Playlist playlist = Globals.main.playlists[playlist_index];
        cover.Texture = playlist_cover;
        playlist_name_label.Text = playlist.name;

        if (playlist.songs != null) track_count_label.Text = $"{playlist.songs.Count} track" + (playlist.songs.Count != 1 ? "s" : "");
        else track_count_label.Text = "0 tracks";

        Update();
    }

    public (int, int) CalculateSize()
    {
        int top = Mathf.Min(Mathf.Max(ScrollVertical - top_card_size_y, 0), GetTotalHeight() - (int)Size.Y - top_card_size_y);
        top -= top % SINGLE_BUTTON_SIZE;

        int bottom = Mathf.Max(GetTotalHeight() - (top + (int)Size.Y) - top_card_size_y, 0);
        bottom -= bottom % SINGLE_BUTTON_SIZE;

        return (top, bottom);
    }

    public int GetFirstVisibleTrack()
    {
        int top = Mathf.Min(Mathf.Max(ScrollVertical - top_card_size_y, 0), GetTotalHeight() - (int)Size.Y - top_card_size_y);
        top -= top % SINGLE_BUTTON_SIZE;
        return Mathf.Max((top) / SINGLE_BUTTON_SIZE, 0);
    }

    public void UpdateTrack(int index, in TrackData track_data, in Texture2D cover)
    {
        int first_visible_track = GetFirstVisibleTrack();
        if (Globals.main.looked_at_playlist != Globals.main.playlist_index || index < first_visible_track || index >= (track_displays.Count + first_visible_track))
            return;

        track_displays[index - first_visible_track].Init(index, track_data, GetPlaylist().type, cover);
    }

    public void Update()
    {
        track_datas.Clear();

        last_first_track = -1;
        last_scroll = -1;
        last_size = Vector2.Zero;

        UpdateTracks(GetPlaylist());
    }
}
