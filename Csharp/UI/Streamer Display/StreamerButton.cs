using Godot;
using System;

public partial class StreamerButton : Button
{
	public bool enabled = false;
	StreamDisplay display;
	[Export] PackedScene stream_display;
	[Export] TextureRect texture;

    public override void _Ready() {
		ButtonUp += Toggle;

		SetFontColors();
	}

	public void Toggle() {
		enabled = !enabled;

		if (enabled) { 
			display = stream_display.Instantiate() as StreamDisplay;
			GetTree().CurrentScene.AddChild(display);

			Globals.main.OnLoadSong += SetStreamDisplayVariables;
			SetStreamDisplayVariables();
        }
		else {
			display.QueueFree();
			Globals.main.OnLoadSong -= SetStreamDisplayVariables;
		}

		SetFontColors();
	}

	private void SetFontColors() {
		if (enabled) {
			AddThemeColorOverride("font_color", Colors.Red);
			AddThemeColorOverride("font_focus_color", Colors.Red);
			AddThemeColorOverride("font_pressed_color", Colors.Red);
			AddThemeColorOverride("font_hover_color", Colors.Red);
			AddThemeColorOverride("font_pressed_color", Colors.Red);
		}
		else {
			AddThemeColorOverride("font_color", Globals.normal_text_color);
			AddThemeColorOverride("font_focus_color", Globals.normal_text_color);
			AddThemeColorOverride("font_pressed_color", Globals.normal_text_color);
			AddThemeColorOverride("font_hover_color", Globals.normal_text_color);
			AddThemeColorOverride("font_pressed_color", Globals.normal_text_color);
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
