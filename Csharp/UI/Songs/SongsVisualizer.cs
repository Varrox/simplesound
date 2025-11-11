using Godot;
using SSLParser;
using Godot.Collections;
using System.Threading;

public partial class SongsVisualizer : ScrollContainer
{
	[Export] public PackedScene Template;
	[Export] public Control container;

    Thread update_songs_thread;

    int last_scroll = 0;

    string[] songs;
    bool album;

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
        if(!album)
        {
            if (last_scroll != ScrollVertical)
            {
                for (int i = 1; i < container.GetChildCount(); i++)
                {
                    SongDisplay child = container.GetChild(i) as SongDisplay;
                    bool hidden = IsHidden(child);

                    child.cover.Visible = !hidden;
                    child.ProcessMode = hidden ? ProcessModeEnum.Disabled : ProcessModeEnum.Inherit;

                    child.cover.Texture = hidden ? null : ConvertToGodot.GetCover(songs[i - 1]);
                }

                last_scroll = ScrollVertical;
            }
        }
    }

    public void Load(int playlist_index, Texture2D playlist_cover)
	{
		Globals.main.current_looked_at_playlist = playlist_index;

        Playlist playlist = Globals.main.playlists[playlist_index];

        album = playlist.Type == Playlist.PlaylistType.Album;

        Array<Node> song_displays = container.GetChildren();

		(song_displays[0].GetChild(0).GetChild(0) as TextureRect).Texture = playlist_cover;
        (song_displays[0].GetChild(1) as Label).Text = playlist.Name;

		if (playlist.Songs != null)
		{
            songs = playlist.Songs.ToArray();

            (song_displays[0].GetChild(2) as Label).Text = $"{playlist.Songs.Count} song" + (playlist.Songs.Count != 1 ? "s" : "");
            song_displays.RemoveAt(0);
        }
		else
		{
			(song_displays[0].GetChild(2) as Label).Text = "0 songs";

            for (int i = 1; i < song_displays.Count; i++)
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
        }

        if (song_displays.Count > songs.Length) // delete overflow
		{
			for(int i = songs.Length; i < song_displays.Count; i++)
			{
                SongDisplay song_display = song_displays[i] as SongDisplay;
                Globals.main.OnPlay -= song_display.SetTextures;
                Globals.main.OnLoadSong -= song_display.SetHighlight;

				song_displays[i].QueueFree();
                song_displays.RemoveAt(i);
				i--;
			}
        }

        update_songs_thread = new Thread(new ThreadStart(() => UpdateAllSongs(ref playlist, ref song_displays, playlist_index)));
        update_songs_thread.Start();
    }

    public void UpdateAllSongs(ref Playlist playlist, ref Array<Node> song_displays, int playlist_index)
    {
        for (int i = 0; i < playlist.Songs.Count; i++) // update all
        {
            //GD.Print("Iteration: ", i, ", Thread: ", Thread.CurrentThread.ManagedThreadId);

            SongDisplay disp = null;

            if (i >= song_displays.Count) // create song display if one does not exist
            {
                disp = Template.Instantiate() as SongDisplay;
                container.CallThreadSafe("add_child", disp);
            }
            else // use ones that already exist
            {
                disp = song_displays[i] as SongDisplay;
            }

            bool hidden = (bool)CallThreadSafe("IsHidden", disp);

            // init the playlist
            disp.Init(Tools.GetMediaTitle(playlist.Songs[i]), Metadata.GetArtist(playlist.Songs[i]), Tools.SecondsToTimestamp(Metadata.GetTotalTime(playlist.Songs[i])), playlist_index, i, Metadata.IsExplicit(playlist.Songs[i]), playlist.Type, !album || !hidden ? ConvertToGodot.GetCover(playlist.Songs[i]) : null);
        }
    }

	public void UpdateSong(int index, string song_name, string artist, string time, bool explicit_lyrics, Texture2D texture)
	{
		if(Globals.main.current_looked_at_playlist == Globals.main.playlist_index)
		{
            SongDisplay disp = container.GetChild(index + 1) as SongDisplay;
            disp.Init(song_name, artist, time, Globals.main.current_looked_at_playlist, index, explicit_lyrics, Globals.main.playlist.Type, texture);
        }
    }
}
