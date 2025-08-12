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
		if(Globals.main.currentPlaylist == Globals.main.currentLookingAtPlaylist)
		{
			Globals.main.Play();
		}
		else
		{
            Globals.main.LoadPlaylist(Globals.main.currentLookingAtPlaylist);
            Globals.main.SetSong(0);
        }
	}

    public override void _Process(double delta)
    {
        Icon = (Globals.main.currentPlaylist == Globals.main.currentLookingAtPlaylist) && Globals.main.playing ? Globals.pause_texture : Globals.play_texture;
    }
}
