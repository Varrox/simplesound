using Godot;
using System;
using System.Collections.Generic;

public partial class SongsVisualizer : ScrollContainer
{
    [Export] public PackedScene template;
    [Export] public VBoxContainer container;
    [Export] public Control top_spacer, bottom_spacer;
    [Export] public TextureRect cover;
    [Export] public Label playlist_name;
    [Export] public Label songs_count;

    int last_scroll = -1, last_first_song = -1;
    Vector2 last_size = Vector2.Zero;

    SongDisplay[] song_displays;
    Dictionary<int, SongData> song_datas;

    const int min_y = 60;
    int separation = 0, top_card_y, single_button_size;
    int GetTotalHeight()
    {
        return (single_button_size * (Globals.main.playlists[Globals.main.looked_at_playlist].songs.Count + 2));
    }

    public override void _Ready()
    {
        top_card_y = (int)(container.GetChild(0) as Control).Size.Y + separation;
        single_button_size = (min_y + separation);

        Globals.main.playlist_visualizer.OnSelectPlaylist += LoadPlaylist;

        song_datas = new Dictionary<int, SongData>();
    }

    public override void _Process(double delta)
    {
        Playlist playlist = Globals.main.playlists[Globals.main.looked_at_playlist];

        if (playlist == null)
            return;
        
        if (last_scroll != ScrollVertical || last_size != Size)
        {
            int max_displays = (int)((int)Size.Y / (float)single_button_size + 1f) + 1;
            (int top, int bottom) = CalculateSize();
            int first_song = GetFirstSong();
            //GD.Print(top, " ", bottom, " ", first_song, " ", max_displays);

            top_spacer.CustomMinimumSize = Vector2.Down * top;
            bottom_spacer.CustomMinimumSize = Vector2.Down * bottom;

            if (last_size != Size)
            {
                if(song_displays == null)
                {
                    song_displays = new SongDisplay[max_displays];
                    for(int i = 0; i < song_displays.Length; i++)
                    {
                        song_displays[i] = template.Instantiate() as SongDisplay;
                        container.AddChild(song_displays[i]);

                        SongData data = new SongData(playlist.songs[first_song + i]);
                        song_displays[i].Init(first_song + i, data, playlist.type, playlist.type == Playlist.PlaylistType.Album ? null : ConvertToGodot.GetCover(playlist.songs[first_song + i]));
                    }
                }
                else if(song_displays.Length != max_displays)
                {
                    if(song_displays.Length < max_displays)
                    {
                        SongDisplay[] displays = new SongDisplay[max_displays];
                        song_displays.CopyTo(displays, 0);
                        song_displays = displays;

                        for(int i = 0; i < song_displays.Length; i++)
                        {
                            if(!container.GetChildren().Contains(song_displays[i]))
                            {
                                song_displays[i] = template.Instantiate() as SongDisplay;
                                container.AddChild(song_displays[i]);
                            }
                        }
                    }
                    else if(song_displays.Length > max_displays)
                    {
                        for(int i = song_displays.Length - max_displays - 1; i >= 0; i--)
                        {
                            song_displays[i].QueueFree();
                        }

                        SongDisplay[] displays = new SongDisplay[max_displays];
                        song_displays.CopyTo(displays, 0);
                        song_displays = displays;
                    }
                }

                container.MoveChild(bottom_spacer, container.GetChildCount() - 1);
            }

            if (first_song != last_first_song)
            {
                bool f = first_song > last_first_song;
                song_datas.Remove(first_song > last_first_song ? last_first_song : first_song + song_datas.Count);
                Texture2D last_cover = null;

                for(int i = !f ? 0 : song_displays.Length - 1; !f ? i < song_displays.Length : i >= 0; i += !f ? 1 : -1)
                {
                    int song = first_song + i;
                    if(!song_datas.ContainsKey(song))
                    {
                        SongData data = new SongData(playlist.songs[song]);
                        song_datas[song] = data;
                        song_displays[i].Init(song, song_datas[song], playlist.type, (playlist.type == Playlist.PlaylistType.Album) ? null : ConvertToGodot.GetCover(playlist.songs[song]));
                        last_cover = song_displays[i].cover.Texture;
                    }
                    else // Swap
                    {
                        Texture2D cover = song_displays[i].cover.Texture;
                        song_displays[i].Init(song, song_datas[song], playlist.type, (playlist.type == Playlist.PlaylistType.Album) ? null : last_cover);
                        last_cover = cover;
                    }
                }

                last_first_song = first_song;
            }

            last_scroll = ScrollVertical;
            last_size = Size;
        }
    }

    private void LoadPlaylist(int playlist_index, Texture2D playlist_cover)
    {
        Playlist playlist = Globals.main.playlists[playlist_index];
        cover.Texture = playlist_cover;
        playlist_name.Text = playlist.name;

        if (playlist.songs != null)
        {
            songs_count.Text = $"{playlist.songs.Count} song" + (playlist.songs.Count != 1 ? "s" : "");
        }
        else
        {
            songs_count.Text = "0 songs";
        }

        song_datas.Clear();

        Update();
    }

    public (int, int) CalculateSize()
    {
        int top = (int)MathF.Min(MathF.Max(ScrollVertical - top_card_y, 0), GetTotalHeight() - (int)Size.Y - top_card_y);
        top -= top % single_button_size;

        int bottom = (int)MathF.Max(GetTotalHeight() - (top + (int)Size.Y) - top_card_y, 0);
        bottom -= bottom % single_button_size;

        return (top, bottom);
    }

    public int GetFirstSong()
    {
        int top = (int)MathF.Min(MathF.Max(ScrollVertical - top_card_y, 0), GetTotalHeight() - (int)Size.Y - top_card_y);
        top -= top % single_button_size;
        return (int)MathF.Max((top) / single_button_size, 0);
    }

    public void UpdateSong(int index, SongData song_data, Texture2D cover)
    {
        int first_song = GetFirstSong();
        if (Globals.main.looked_at_playlist != Globals.main.playlist_index || index < first_song || index >= (song_displays.Length + first_song))
            return;

        song_displays[index - first_song].Init(index, song_data, Globals.main.playlists[Globals.main.looked_at_playlist].type, cover);
    }

    public void Update()
    {
        last_scroll = -1;
        last_first_song = -1;
    }
}
