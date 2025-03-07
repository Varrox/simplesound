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

		if(playlist == main.currentPlaylist) Playlist = main.playlist;
		else Playlist = SaveSystem.LoadPlaylist(main.playlists[playlist]);

		var SongDisplays = container.GetChildren();
		(SongDisplays[0].GetChild(0).GetChild(0) as TextureRect).Texture = playDisp;
        (SongDisplays[0].GetChild(1) as Label).Text = Playlist.Name;
		(SongDisplays[0].GetChild(2) as Label).Text = $"{Playlist.Songs.Count} song" + (Playlist.Songs.Count != 1 ? "s" : "");
        SongDisplays.RemoveAt(0);

		if(SongDisplays.Count > Playlist.Songs.Count) // delete overflow
		{
			for(int i = Playlist.Songs.Count; i < SongDisplays.Count; i++)
			{
				var ds = SongDisplays[i] as Songdisplay;
                main.OnPlay -= ds.SetTextures;
                main.OnLoadSong -= ds.SetHighlight;
				SongDisplays[i].QueueFree();
                SongDisplays.RemoveAt(i);
				i--;
			}
		}

        for (int i = 0; i < Playlist.Songs.Count; i++) // update all
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
            disp.init(Tools.GetMediaTitle(Playlist.Songs[i]), Metadata.GetArtist(Playlist.Songs[i]), Tools.SecondsToTimestamp(Metadata.GetTotalTime(Playlist.Songs[i])), playlist, i, Metadata.IsExplicit(Playlist.Songs[i]), this, Playlist.type != Playlist.PlaylistType.Album ? ConvertToGodot.getCover(Playlist.Songs[i]) : null, menu);
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
