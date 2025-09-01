using Godot;
using Godot.Collections;
using SSLParser;
using System;

public partial class PlaylistsVisualizer : ScrollContainer
{
	[Export] public PackedScene template;
    [Export] public VBoxContainer container;
    [Export] public Control moreMenu;

    public Action<int, Texture2D> OnSelectPlaylist;

    public override void _Process(double delta)
    {
        for (int i = 0; i < container.GetChildCount(); i++)
        {
            if (container.GetChild(i) is PlaylistDisplay) 
            {
                PlaylistDisplay display = (container.GetChild(i) as PlaylistDisplay);
                display.Cover.Visible = container.GetGlobalRect().Intersects(display.GetGlobalRect());
            }
        }
    }

    public void LoadAllPlaylistVisuals() // only done once for init
    {
        for (int i = 0; i < Globals.main.playlists.Count; i++)
        {
            LoadPlaylist(i);
        }
    }

    public void LoadPlaylist(int i)
    {
        PlaylistDisplay playlist = template.Instantiate() as PlaylistDisplay;
        container.AddChild(playlist);
        LoadDataIntoPlaylist(i, playlist, i == Globals.main.current_looked_at_playlist);
    }

    public void LoadDataIntoPlaylist(int i, PlaylistDisplay playlist, bool current)
    {
        playlist.Init(i == Globals.main.playlist_index ? Globals.main.playlist : MainParser.ParsePlaylist(Globals.main.playlists[i]), i, current, moreMenu);
    }

    public void UpdatePlaylists()
    {
        Array<Node> playlists = container.GetChildren();
        int end = playlists.Count;

        if (playlists.Count < Globals.main.playlists.Count) // less
        {
            for(int i = playlists.Count; i < Globals.main.playlists.Count; i++)
            {
                LoadPlaylist(i);
            }
        }
        else if (playlists.Count > Globals.main.playlists.Count) // more
        {
            for (int i = playlists.Count; i < Globals.main.playlists.Count; i++) // delete the extra
            {
                playlists[i].QueueFree();
                playlists.RemoveAt(i);
            }

            end = Globals.main.playlists.Count;
        }

        for(int i = 0; i < end; i++)
        {
            LoadDataIntoPlaylist(i, playlists[i] as PlaylistDisplay, i == Globals.main.playlist_index);
        }
    }
}
