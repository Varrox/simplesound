using Godot;
using SSLParser;
using Godot.Collections;
using System.Threading;

public partial class SongsVisualizer : ScrollContainer
{
	[Export] public PackedScene Template;
	[Export] public Control menu, container;

    Thread update_songs_thread;

    int lastScroll = 0;

    string[] songs;
    bool album;

	public override void _Ready()
	{
		Globals.main.playlistVisualizer.OnSelectPlaylist += Load;
	}

    public bool IsHidden(Songdisplay songdisplay)
    {
        return !songdisplay.GetGlobalRect().Intersects(GetRect());
    }

    public override void _Process(double delta)
    {
        if(!album)
        {
            if (lastScroll != ScrollVertical)
            {
                for (int i = 1; i < container.GetChildCount(); i++)
                {
                    Songdisplay child = container.GetChild(i) as Songdisplay;
                    bool hidden = IsHidden(child);

                    child.Cover.Visible = !hidden;
                    child.ProcessMode = hidden ? ProcessModeEnum.Disabled : ProcessModeEnum.Inherit;

                    child.Cover.Texture = hidden ? null : ConvertToGodot.GetCover(songs[i - 1]);
                }

                lastScroll = ScrollVertical;
            }
        }
    }

    public void Load(int playlist, Texture2D playDisp)
	{
		Globals.main.currentLookingAtPlaylist = playlist;

        Playlist Playlist = null;

        if (playlist == Globals.main.currentPlaylist) Playlist = Globals.main.playlist;
		else Playlist = MainParser.ParsePlaylist(Globals.main.playlists[playlist]);

        album = Playlist.Type == Playlist.PlaylistType.Album;

        Array<Node> SongDisplays = container.GetChildren();

		(SongDisplays[0].GetChild(0).GetChild(0) as TextureRect).Texture = playDisp;
        (SongDisplays[0].GetChild(1) as Label).Text = Playlist.Name;

		if (Playlist.Songs != null)
		{
            songs = Playlist.Songs.ToArray();

            (SongDisplays[0].GetChild(2) as Label).Text = $"{Playlist.Songs.Count} song" + (Playlist.Songs.Count != 1 ? "s" : "");
            SongDisplays.RemoveAt(0);
        }
		else
		{
			(SongDisplays[0].GetChild(2) as Label).Text = "0 songs";

            for (int i = 1; i < SongDisplays.Count; i++)
            {
                Songdisplay display = SongDisplays[i] as Songdisplay;
                Globals.main.OnPlay -= display.SetTextures;
                Globals.main.OnLoadSong -= display.SetHighlight;
                SongDisplays[i].QueueFree();
            }

            return;
        }

		if(SongDisplays.Count > songs.Length) // delete overflow
		{
			for(int i = songs.Length; i < SongDisplays.Count; i++)
			{
                Songdisplay display = SongDisplays[i] as Songdisplay;
                Globals.main.OnPlay -= display.SetTextures;
                Globals.main.OnLoadSong -= display.SetHighlight;
				SongDisplays[i].QueueFree();
                SongDisplays.RemoveAt(i);
				i--;
			}
        }

        if (update_songs_thread != null)
        {
            update_songs_thread.Join();
        }

        update_songs_thread = new Thread(new ThreadStart(() => UpdateAllSongs(ref Playlist, ref SongDisplays, playlist)));
        update_songs_thread.Start();
    }

    public void UpdateAllSongs(ref Playlist Playlist, ref Array<Node> SongDisplays, int playlist)
    {
        for (int i = 0; i < Playlist.Songs.Count; i++) // update all
        {
            //GD.Print("Iteration: ", i, ", Thread: ", Thread.CurrentThread.ManagedThreadId);

            Songdisplay disp = null;

            if (i >= SongDisplays.Count) // create song display if one does not exist
            {
                disp = Template.Instantiate() as Songdisplay;
                container.CallThreadSafe("add_child", disp);
            }
            else // use ones that already exist
            {
                disp = SongDisplays[i] as Songdisplay;
            }

            bool hidden = (bool)CallThreadSafe("IsHidden", disp);

            // init the playlist
            disp.Init(Tools.GetMediaTitle(Playlist.Songs[i]), Metadata.GetArtist(Playlist.Songs[i]), Tools.SecondsToTimestamp(Metadata.GetTotalTime(Playlist.Songs[i])), playlist, i, Metadata.IsExplicit(Playlist.Songs[i]), Playlist.Type, !album || !hidden ? ConvertToGodot.GetCover(Playlist.Songs[i]) : null, menu);
        }
    }

	public void UpdateSong(int index, string sname, string artist, string time, bool explicitlyrics, Texture2D texture)
	{
		if(Globals.main.currentLookingAtPlaylist == Globals.main.currentPlaylist)
		{
            Songdisplay disp = container.GetChild(index + 1) as Songdisplay;
            disp.Init(sname, artist, time, Globals.main.currentLookingAtPlaylist, index, explicitlyrics, Globals.main.playlist.Type, texture, menu);
        }
    }
}
