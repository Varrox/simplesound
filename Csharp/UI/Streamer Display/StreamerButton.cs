using Godot;
using System;

public partial class StreamerButton : Button
{
	bool enabled = false;
	StreamDisplay display;
	[Export] PackedScene stream_display;
	[Export] TextureRect texture;

    public override void _Ready()
	{
		ButtonUp += Toggle;
	}

	public void Toggle()
	{
		enabled = !enabled;

		if (enabled)
		{ 
			display = stream_display.Instantiate() as StreamDisplay;
			GetTree().CurrentScene.AddChild(display);

			Globals.main.OnLoadSong += SetStreamDisplayVariables;
			SetStreamDisplayVariables();
        }
		else
		{
			display.QueueFree();
			Globals.main.OnLoadSong -= SetStreamDisplayVariables;
		}
	}

	public void SetStreamDisplayVariables()
	{
		display.cover_art.Texture = Globals.player.SongCover.Texture;
		display.song.Text = Globals.player.SongName.Text;
		display.artist.Text = Globals.player.SongArtist.Text;
		display.background.Texture = texture.Texture;
    }
}
