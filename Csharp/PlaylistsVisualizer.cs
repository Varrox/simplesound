using Godot;
using Godot.Collections;

public partial class PlaylistsVisualizer : Node
{
	[Export] public PackedScene template;
	[Export] public Main mainController;
    [Export] public VBoxContainer container;
    [Export] public Control moreMenu;
    [Export] public Texture2D defaultCover;

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
        LoadDataIntoPlaylist(i, playlist, i == mainController.currentLookingAtPlaylist);
    }

    public void LoadDataIntoPlaylist(int i, PlaylistDisplay playlist, bool current)
    {
        playlist.init(SaveSystem.LoadPlaylist(mainController.playlists[i]), i, this, current, moreMenu);
    }

    public void UpdatePlaylists()
    {
        Array<Node> playlists = container.GetChildren();
        int end = playlists.Count;
        if(playlists.Count != mainController.playlists.Length) // not length
        {
            if (playlists.Count < mainController.playlists.Length) // less
            {
                for(int i = playlists.Count; i < mainController.playlists.Length - playlists.Count; i++)
                {
                    LoadPlaylist(i);
                }
            }
            else // same or more
            {
                for (int i = playlists.Count; i < playlists.Count - mainController.playlists.Length; i++) // delete the extra
                {
                    playlists[i].QueueFree();
                    playlists.RemoveAt(i);
                }
                end = playlists.Count;
            }
        }

        for(int i = 0; i < end; i++)
        {
            LoadDataIntoPlaylist(i, playlists[i] as PlaylistDisplay, i == mainController.currentPlaylist);
        }
    }
}
