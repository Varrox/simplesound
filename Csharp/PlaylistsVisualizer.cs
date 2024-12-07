using Godot;
using Godot.Collections;

public partial class PlaylistsVisualizer : Node
{
	[Export] public PackedScene template;
	[Export] public Main mainController;
    [Export] public VBoxContainer container;

    public void LoadAllPlaylistVisuals() // only done once for init
    {
        for (int i = 0; i < mainController.playlists.Length; i++)
        {
            LoadPlaylist(i);
        }
    }

    public void LoadPlaylist(int i)
    {
        Control playlist = (Control)template.Instantiate();
        container.AddChild(playlist);
        LoadDataIntoPlaylist(i, playlist);
    }

    public void LoadDataIntoPlaylist(int i, Node playlist)
    {
        SaveSystem.GetPlaylistAttributes(mainController.playlists[i], out string name, out string coverpath, out int songcount);
        playlist.Call("init", name, coverpath, songcount, i);
    }

    public void UpdatePlaylists()
    {
        Array<Node> playlists = container.GetChildren();
        int end = playlists.Count;
        if(playlists.Count != mainController.playlists.Length)
        {
            if (playlists.Count < mainController.playlists.Length)
            {
                for(int i = playlists.Count; i < mainController.playlists.Length - playlists.Count; i++)
                {
                    LoadPlaylist(i);
                }
            }
            else
            {
                for (int i = playlists.Count; i < playlists.Count - mainController.playlists.Length; i++)
                {
                    playlists[i].QueueFree();
                    playlists.RemoveAt(i);
                }
                end = playlists.Count;
            }
        }

        for(int i = 0; i < end; i++)
        {
            LoadDataIntoPlaylist(i, playlists[i]);
        }
    }
}
