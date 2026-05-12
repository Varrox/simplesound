using Godot;
using System.Collections.Generic;

public partial class SongsVisualizer : ScrollContainer
{
    [Export] public PackedScene template;
    [Export] public Control top_card;
    [Export] public VBoxContainer container; // Only children will be song displays. Is a sub VBoxContainer of the one containing top and bottom spacers, and top card.
    [Export] public Control top_spacer, bottom_spacer;
    [Export] public TextureRect cover;
    [Export] public Label playlist_name;
    [Export] public Label songs_count;

    int last_scroll = -1, last_first_song = -1;
    Vector2 last_size = Vector2.Zero;

    List<SongDisplay> song_displays;
    Dictionary<int, SongData> song_datas;

    const int min_y = 60;
    int separation = 0, top_card_y, single_button_size;
    
    int GetTotalHeight()
    {
        return (single_button_size * Globals.main.playlists[Globals.main.looked_at_playlist].songs.Count);
    }

    public override void _Ready()
    {
        top_card_y = (int)top_card.Size.Y + separation;
        single_button_size = (min_y + separation);

        Globals.main.playlist_visualizer.OnSelectPlaylist += LoadPlaylist;

        song_datas = new Dictionary<int, SongData>();
        song_displays = new List<SongDisplay>();
    }

    public override void _Process(double delta)
    {
        Playlist playlist = Globals.main.playlists[Globals.main.looked_at_playlist];

        if (playlist == null)
            return;
        
        if (last_scroll != ScrollVertical || last_size != Size)
        {
            int max_displays = (int)Mathf.Ceil(Size.Y / single_button_size) + 3;
            int target_displays = Mathf.Min(max_displays, playlist.songs.Count);
            (int top, int bottom) = CalculateSize();
            int first_song = GetFirstSong();

            top_spacer.CustomMinimumSize = Vector2.Down * top;
            bottom_spacer.CustomMinimumSize = Vector2.Down * bottom;
            
            bottom_spacer.Visible = target_displays == max_displays;
            top_spacer.Visible = target_displays == max_displays;

            // Check if there is enough displays, if not, add or remove them. when adding a new one, update it.

            if(song_displays.Count != target_displays) // NOTE: Is only true if there is more songs added (only when previous count was below max_displays), or the size changed.
            {
                if(song_displays.Count > target_displays) // Delete old
                {
                    for(int i = song_displays.Count; i >= target_displays; i--)
                    {
                        song_displays[i].QueueFree();
                        song_displays.RemoveAt(i);
                    }
                }
                else // Add new
                {
                    for(int i = song_displays.Count; i < target_displays; i++)
                    {
                        song_displays.Add(template.Instantiate() as SongDisplay);
                        container.AddChild(song_displays[i]);

                        int song = first_song + i;

                        SongData data = GetSongData(song, playlist);
                        song_displays[i].Init(song, data, playlist.type, (playlist.type == Playlist.PlaylistType.Album) ? null : ConvertToGodot.GetCover(playlist.songs[song]));
                    }
                }
            }

            // If first song has changed, go through and update only the ones that need new covers and data, and shift back old displays.

            int shift = first_song - last_first_song; // The amount to shift by
            int a_shift = Mathf.Abs(shift);

            if(shift != 0 && ScrollVertical > top_card_y + single_button_size || a_shift < song_displays.Count) // NOTE: Is only true if there has been any scrolling, and past the beginning.
            {
                // If shift is less than 0, then the user has scrolled up.
                // If shift is more than 0, then the user has scrolled down.

                // Scrolling up, requires the last song display to be placed at the beginning, and updated.
                // Scrolling down, requires the first song display to be placed at the end, and updated.

                // Do these ^^^ (|shift|) times

                int last_index = song_displays.Count - 1;

                for(int i = 0; i < a_shift; i++)
                {
                    SongDisplay display;
                    int index;

                    if(shift < 0)
                    {
                        display = song_displays[last_index];
                        song_displays.RemoveAt(last_index);
                        song_displays.Insert(0, display);

                        container.MoveChild(display, 0);

                        index = (a_shift - i) - 1;
                    }
                    else
                    {
                        display = song_displays[0];
                        song_displays.RemoveAt(0);
                        song_displays.Add(display); // Add instead of insert because it is probably faster and makes more sense logic wise.

                        container.MoveChild(display, last_index);

                        index = song_displays.Count - (a_shift - i);
                    }

                    int song = first_song + index;

                    SongData data = GetSongData(song, playlist);
                    song_displays[index].Init(song, data, playlist.type, (playlist.type == Playlist.PlaylistType.Album) ? null : ConvertToGodot.GetCover(playlist.songs[song]));
                }
            }
            

            last_scroll = ScrollVertical;
            last_size = Size;
            last_first_song = first_song;
        }
    }

    private SongData GetSongData(int song, in Playlist playlist)
    {
        if (!song_datas.ContainsKey(song)) {
            if(song < playlist.songs.Count) {
                SongData data = new SongData(playlist.songs[song]);
                song_datas[song] = data;
            }
            else song_datas[song] = new SongData{};
        }
        return song_datas[song];
    }

    private void LoadPlaylist(int playlist_index, Texture2D playlist_cover)
    {
        Playlist playlist = Globals.main.playlists[playlist_index];
        cover.Texture = playlist_cover;
        playlist_name.Text = playlist.name;

        if (playlist.songs != null) songs_count.Text = $"{playlist.songs.Count} song" + (playlist.songs.Count != 1 ? "s" : "");
        else songs_count.Text = "0 songs";

        Update();
    }

    public (int, int) CalculateSize()
    {
        int top = Mathf.Min(Mathf.Max(ScrollVertical - top_card_y, 0), GetTotalHeight() - (int)Size.Y - top_card_y);
        top -= top % single_button_size;

        int bottom = Mathf.Max(GetTotalHeight() - (top + (int)Size.Y) - top_card_y, 0);
        bottom -= bottom % single_button_size;

        return (top, bottom);
    }

    public int GetFirstSong()
    {
        int top = Mathf.Min(Mathf.Max(ScrollVertical - top_card_y, 0), GetTotalHeight() - (int)Size.Y - top_card_y);
        top -= top % single_button_size;
        return Mathf.Max((top) / single_button_size, 0);
    }

    public void UpdateSong(int index, in SongData song_data, in Texture2D cover)
    {
        int first_song = GetFirstSong();
        if (Globals.main.looked_at_playlist != Globals.main.playlist_index || index < first_song || index >= (song_displays.Count + first_song))
            return;

        song_displays[index - first_song].Init(index, song_data, Globals.main.playlists[Globals.main.looked_at_playlist].type, cover);
    }

    public void Update()
    {
        song_datas.Clear();
        last_scroll = -1;
        last_first_song = -1;
    }
}
