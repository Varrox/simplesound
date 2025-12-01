using Godot;
using Godot.Collections;
using System;

public partial class PlaylistsVisualizer : ScrollContainer
{
	[Export] public PackedScene template;
    [Export] public VBoxContainer container;

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
        for (int i = 0; i < Globals.main.playlist_paths.Count; i++)
        {
            LoadPlaylist(i);
        }
    }

    public void LoadPlaylist(int i)
    {
        PlaylistDisplay playlist = template.Instantiate() as PlaylistDisplay;
        container.AddChild(playlist);
        LoadDataIntoPlaylist(i, playlist);
    }

    public void LoadDataIntoPlaylist(int i, PlaylistDisplay playlist)
    {
        playlist.Init(Globals.main.playlists[i], i);
    }

    public void UpdatePlaylists()
    {
        Array<Node> playlist_buttons = container.GetChildren();
        int end = playlist_buttons.Count;

        if (playlist_buttons.Count < Globals.main.playlists.Count) // less
        {
            for(int i = playlist_buttons.Count; i < Globals.main.playlists.Count; i++)
            {
                LoadPlaylist(i);
            }
        }
        else if (playlist_buttons.Count > Globals.main.playlists.Count) // more
        {
            for (int i = playlist_buttons.Count; i < Globals.main.playlists.Count; i++) // delete the extra
            {
                playlist_buttons[i].QueueFree();
                playlist_buttons.RemoveAt(i);
            }

            end = Globals.main.playlists.Count;
        }

        for(int i = 0; i < end; i++)
        {
            LoadDataIntoPlaylist(i, playlist_buttons[i] as PlaylistDisplay);
        }
    }
}
