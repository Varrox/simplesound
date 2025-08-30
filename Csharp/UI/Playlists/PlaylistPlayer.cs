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
		if(Globals.main.current_playlist == Globals.main.current_looked_at_playlist)
		{
			Globals.main.Play();
		}
		else
		{
            Globals.main.LoadPlaylist(Globals.main.current_looked_at_playlist);
            Globals.main.SetSong(0);
        }
	}

    public override void _Process(double delta)
    {
        Icon = (Globals.main.current_playlist == Globals.main.current_looked_at_playlist) && Globals.main.playing ? Globals.pause_texture : Globals.play_texture;
    }
}
