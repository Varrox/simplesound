using Godot;
using Godot.Collections;

public partial class PlaylistsVisualizer : Node
{
	[Export] public PackedScene template;
	[Export] public Main mainController;
    [Export] public VBoxContainer container;

    [Signal] public delegate void OnSelectPlaylistEventHandler(int playlist, Texture2D img);

    public void LoadAllPlaylistVisuals() // only done once for init
    {
        for (int i = 0; i < mainController.playlists.Length; i++)
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
        SaveSystem.GetPlaylistAttributes(mainController.playlists[i], out string name, out string coverpath, out int songcount);
        playlist.init(name, coverpath, songcount, i, this);
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
            LoadDataIntoPlaylist(i, playlists[i] as PlaylistDisplay);
        }
    }
}
