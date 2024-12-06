using Godot;
using Godot.Collections;

public partial class PlaylistsVisualizer : Node
{
	[Export] public PackedScene template;
	[Export] public Main mainController;

    float space = 20;

    public void LoadAllPlaylistVisuals() // only done once for init
    {
        for (int i = 0; i < mainController.playlists.Length; i++)
        {
            LoadPlaylist(i, new Vector2(20, space));
            space += 95;
        }
    }

    public void LoadPlaylist(int i, Vector2 position)
    {
        Control playlist = (Control)template.Instantiate();
        AddChild(playlist);
        playlist.Position = position;
        LoadDataIntoPlaylist(i, playlist);
    }

    public void LoadDataIntoPlaylist(int i, Node playlist)
    {
        SaveSystem.GetPlaylistAttributes(mainController.playlists[i], out string name, out string coverpath, out int songcount);
        playlist.Call("init", name, coverpath, songcount, i, mainController);
    }

    public void UpdatePlaylists()
    {
        Array<Node> playlists = GetChildren();
        int end = playlists.Count;
        if(playlists.Count != mainController.playlists.Length)
        {
            if (playlists.Count < mainController.playlists.Length)
            {
                for(int i = playlists.Count; i < mainController.playlists.Length - playlists.Count; i++)
                {
                    LoadPlaylist(i, new Vector2(20, space));
                    space += 95;
                }
            }
            else
            {
                for (int i = playlists.Count; i < playlists.Count - mainController.playlists.Length; i++)
                {
                    playlists[i].QueueFree();
                    playlists.RemoveAt(i);
                    space -= 95;
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
