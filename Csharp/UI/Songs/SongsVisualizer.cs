using Godot;
using Godot.Collections;
using System.Threading;

public partial class SongsVisualizer : ScrollContainer
{
    [Export] public PackedScene Template;
    [Export] public Control container;
    [Export] public TextureRect cover;
    [Export] public Label playlist_name;
    [Export] public Label songs_count;

    Thread update_songs_thread;

    int last_scroll = -1;

    Playlist playlist
    {
        get
        {
            return Globals.main.playlists[Globals.main.looked_at_playlist];
        }
    }

    public override void _Ready()
    {
        Globals.main.playlist_visualizer.OnSelectPlaylist += Load;
    }

    public bool IsHidden(SongDisplay songdisplay)
    {
        return !songdisplay.GetGlobalRect().Intersects(GetRect());
    }

    public override void _Process(double delta)
    {
        if (playlist == null)
            return;

        if (playlist.type != Playlist.PlaylistType.Album)
        {
            if (last_scroll != ScrollVertical)
            {
                for (int i = 1; i < container.GetChildCount(); i++)
                {
                    SongDisplay child = container.GetChild(i) as SongDisplay;
                    bool hidden = IsHidden(child);

                    child.is_visible = !hidden;
                    child.cover.Visible = !hidden;
                    child.ProcessMode = hidden ? ProcessModeEnum.Disabled : ProcessModeEnum.Inherit;

                    child.cover.Texture = hidden ? null : ConvertToGodot.GetCover(playlist.songs[i - 1]);
                }

                last_scroll = ScrollVertical;
            }
        }
    }

    public void Load(int playlist_index, Texture2D playlist_cover)
    {
        Globals.main.looked_at_playlist = playlist_index;

        Array<Node> song_displays = container.GetChildren();
        song_displays.RemoveAt(0);

        cover.Texture = playlist_cover;
        playlist_name.Text = playlist.name;

        if (playlist.songs != null)
        {
            songs_count.Text = $"{playlist.songs.Count} song" + (playlist.songs.Count != 1 ? "s" : "");
        }
        else
        {
            songs_count.Text = "0 songs";

            for (int i = 0; i < song_displays.Count; i++)
            {
                SongDisplay display = song_displays[i] as SongDisplay;
                Globals.main.OnPlay -= display.SetTextures;
                Globals.main.OnLoadSong -= display.SetHighlight;
                song_displays[i].QueueFree();
            }

            return;
        }

        if (update_songs_thread != null)
        {
            update_songs_thread.Join();
            update_songs_thread = null;
        }

        if (song_displays.Count > playlist.songs.Count) // delete overflow
        {
            for (int i = playlist.songs.Count; i < song_displays.Count; i++)
            {
                SongDisplay song_display = song_displays[i] as SongDisplay;
                Globals.main.OnPlay -= song_display.SetTextures;
                Globals.main.OnLoadSong -= song_display.SetHighlight;

                song_displays[i].QueueFree();
                song_displays.RemoveAt(i);
                i--;
            }
        }

        update_songs_thread = new Thread(new ThreadStart(() => UpdateAllSongs(ref song_displays)));
        update_songs_thread.Start();
    }

    public void UpdateAllSongs(ref Array<Node> song_displays)
    {
        for (int i = 0; i < playlist.songs.Count; i++) // update all
        {
            SongDisplay disp = null;

            if (i >= song_displays.Count) // create song display if one does not exist
            {
                disp = (SongDisplay)Template.Instantiate();
                container.CallThreadSafe("add_child", disp);
            }
            else // use ones that already exist
            {
                disp = song_displays[i] as SongDisplay;
            }

            bool hidden = (bool)CallThreadSafe("IsHidden", disp);

            // init the playlist
            disp.Init(Tools.GetMediaTitle(playlist.songs[i]), Metadata.GetArtist(playlist.songs[i]), Tools.SecondsToTimestamp(Metadata.GetTotalTime(playlist.songs[i])), i, Metadata.IsExplicit(playlist.songs[i]), playlist.type, playlist.type != Playlist.PlaylistType.Album || !hidden ? ConvertToGodot.GetCover(playlist.songs[i]) : null);
        }
    }

    public void UpdateSong(int index, string song_name, string artist, string time, bool explicit_lyrics, Texture2D texture)
    {
        if (Globals.main.looked_at_playlist == Globals.main.playlist_index)
        {
            SongDisplay disp = container.GetChild(index + 1) as SongDisplay;
            disp.Init(song_name, artist, time, index, explicit_lyrics, Globals.main.playlist.type, texture);
        }
    }

    public void RemoveSong(int index)
    {
        SongDisplay song_display = container.GetChild(index + 1) as SongDisplay;
        Globals.main.OnPlay -= song_display.SetTextures;
        Globals.main.OnLoadSong -= song_display.SetHighlight;

        song_display.QueueFree();
    }
}
