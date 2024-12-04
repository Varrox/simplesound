using Godot;
using System;

public partial class PlaylistsVisualizer : Control
{
	[Export] public PackedScene template;
	[Export] public Main mainController;

    public void LoadAllPlaylistVisuals()
    {
        float space = 20;
        for (int i = 0; i < mainController.playlists.Length; i++)
        {
            var pl = SaveSystem.LoadPlaylist(mainController.playlists[i]);
            Control playlist = (Control)template.Instantiate();
            AddChild(playlist);
            playlist.Position = new Vector2(20, space);
            playlist.Call("init", pl.Name, pl.Coverpath, pl.songs.Count, i, mainController);
            space += 100;
        }
    }
}
