using Godot;
using Godot.Collections;
using SSLParser;

public partial class PlaylistsVisualizer : ScrollContainer
{
	[Export] public PackedScene template;
    [Export] public VBoxContainer container;
    [Export] public Control moreMenu;

    [Signal] public delegate void OnSelectPlaylistEventHandler(int playlist, Texture2D img);

    public void LoadAllPlaylistVisuals() // only done once for init
    {
        for (int i = 0; i < Globals.main.playlists.Length; i++)
        {
            LoadPlaylist(i);
        }
    }

    public void LoadPlaylist(int i)
    {
        PlaylistDisplay playlist = template.Instantiate() as PlaylistDisplay;
        container.AddChild(playlist);
        LoadDataIntoPlaylist(i, playlist, i == Globals.main.currentLookingAtPlaylist);
    }

    public void LoadDataIntoPlaylist(int i, PlaylistDisplay playlist, bool current)
    {
        playlist.init(i == Globals.main.currentPlaylist ? Globals.main.playlist : MainParser.ParsePlaylist(Globals.main.playlists[i]), i, this, current, moreMenu);
    }

    public void UpdatePlaylists()
    {
        Array<Node> playlists = container.GetChildren();
        int end = playlists.Count;

        if (playlists.Count < Globals.main.playlists.Length) // less
        {
            for(int i = playlists.Count; i < Globals.main.playlists.Length; i++)
            {
                LoadPlaylist(i);
            }
        }
        else if (playlists.Count > Globals.main.playlists.Length) // more
        {
            for (int i = playlists.Count; i < Globals.main.playlists.Length; i++) // delete the extra
            {
                playlists[i].QueueFree();
                playlists.RemoveAt(i);
            }

            end = playlists.Count;
        }

        for(int i = 0; i < end; i++)
        {
            LoadDataIntoPlaylist(i, playlists[i] as PlaylistDisplay, i == Globals.main.currentPlaylist);
        }

        //scrollContainer.ScrollVertical = 4;
    }
}
