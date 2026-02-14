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
		display.cover_art.Texture = Globals.player.song_cover.Texture;
		display.song.Text = Globals.player.song_name.Text;
		display.artist.Text = Globals.player.song_artist.Text;
		display.background.Texture = texture.Texture;
    }
}
