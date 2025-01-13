using Godot;
using System.IO;

public partial class SongsVisualizer : Control
{
	[Export] public PackedScene Template;
    [Export] public Main main;
    [Export] public PlaylistsVisualizer PlaylistsVisualizer;
	[Export] public Control menu, container;
    [Export] public Texture2D PlayTexture, Pause;
    [Export] public Color highlight;

	public Playlist Playlist;

	public override void _Ready()
	{
		PlaylistsVisualizer.OnSelectPlaylist += Load;
	}

	public void Load(int playlist, Texture2D playDisp)
	{
		main.currentLookingAtPlaylist = playlist;
		Playlist = SaveSystem.LoadPlaylist(main.playlists[playlist]);
		var SongDisplays = container.GetChildren();
		(SongDisplays[0].GetChild(0).GetChild(0) as TextureRect).Texture = playDisp;
        (SongDisplays[0].GetChild(1) as Label).Text = Playlist.Name;
		(SongDisplays[0].GetChild(2) as Label).Text = $"{Playlist.songs.Count} song" + (Playlist.songs.Count != 1 ? "s" : "");
        SongDisplays.RemoveAt(0);

		if(SongDisplays.Count > Playlist.songs.Count) // delete overflow
		{
			for(int i = Playlist.songs.Count; i < SongDisplays.Count; i++)
			{
				var ds = SongDisplays[i] as Songdisplay;
				if(ds.isPlaying)
				{
                    main.OnPlay -= ds.SetTextures;
                }
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
            disp.init(SaveSystem.GetName(Playlist.songs[i]), Metadata.GetArtist(Playlist.songs[i]), SaveSystem.GetTimeFromSeconds(Metadata.GetTotalTime(Playlist.songs[i])), playlist, i, Metadata.IsExplicit(Playlist.songs[i]), this, Playlist.type != Playlist.PlaylistType.Album ? ConvertToGodot.getCover(Playlist.songs[i]) : null, menu);
        }
	}

	public void UpdateSong(int index, string sname, string artist, string time, bool explicitlyrics, Texture2D texture)
	{
		if(main.currentLookingAtPlaylist == main.currentPlaylist)
		{
            Songdisplay disp = container.GetChild(index + 1) as Songdisplay;
            disp.init(sname, artist, time, main.currentLookingAtPlaylist, index, explicitlyrics, this, texture, menu);
        }
    }
}
