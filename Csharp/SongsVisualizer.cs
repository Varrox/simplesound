using Godot;

public partial class SongsVisualizer : Control
{
	[Export] public PackedScene Template;
    [Export] public Main main;
	[Export] public PlaylistsVisualizer PlaylistsVisualizer;
	[Export] public Control menu;
	[Export] public Control container;

	public int currentPlaylist;

	public override void _Ready()
	{
		PlaylistsVisualizer.OnSelectPlaylist += Load;
	}

	public void Load(int playlist, Texture2D playDisp)
	{
		currentPlaylist = playlist;
		string path = main.playlists[playlist];
		var Playlist = SaveSystem.LoadPlaylist(path);
		var SongDisplays = container.GetChildren();
		(SongDisplays[0].GetChild(0) as TextureRect).Texture = playDisp;
        (SongDisplays[0].GetChild(1) as Label).Text = Playlist.Name;
		(SongDisplays[0].GetChild(2) as Label).Text = $"{Playlist.songs.Count} songs";
        SongDisplays.RemoveAt(0);

		if(SongDisplays.Count > Playlist.songs.Count) // delete overflow
		{
			for(int i = Playlist.songs.Count; i < SongDisplays.Count; i++)
			{
				var ds = SongDisplays[i] as Songdisplay;
				main.OnPlay -= ds.SetTextures;
                main.OnLoadSong -= ds.SetHighlight;
				SongDisplays[i].QueueFree();
			}
		}

        for (int i = 0; i < Playlist.songs.Count; i++) // update all
		{
			Songdisplay disp = null;
			if(i >= SongDisplays.Count) // create song display if one does not exist
			{
				disp = Template.Instantiate() as Songdisplay;
                container.AddChild(disp);
            }
			else // use ones that already exist
			{
				disp = SongDisplays[i] as Songdisplay;
            }

			// init the playlist
            disp.init(SaveSystem.GetName(Playlist.songs[i]), Metadata.GetArtist(Playlist.songs[i]), SaveSystem.GetTimeFromSeconds(Metadata.GetTotalTime(Playlist.songs[i])), playlist, i, ConvertToGodot.getCover(Playlist.songs[i]), menu);
        }
	}
}
