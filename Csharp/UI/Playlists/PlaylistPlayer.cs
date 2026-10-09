using Godot;
using System;

public partial class PlaylistPlayer : Button
{
	public override void _Ready()
	{
		ButtonUp += Play;
	}

	public void Play() 
	{ 
		if(Globals.main.playlist_index == Globals.main.looked_at_playlist)
		{
			Globals.main.FlipPlayingState();
		}
		else
		{
            Globals.main.LoadPlaylist(Globals.main.looked_at_playlist);
            Globals.main.SetTrack(0);
        }
	}

    public override void _Process(double delta)
    {
        Icon = ApplicationManager.theme.GetIcon((Globals.main.playlist_index == Globals.main.looked_at_playlist) && Globals.main.playing ? "pause_icon" : "play_icon", Constants.THEME_TYPE);
    }
}
