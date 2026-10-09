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

			Globals.main.OnLoadTrack += SetStreamDisplayVariables;
			SetStreamDisplayVariables();
        }
		else {
			display.QueueFree();
			Globals.main.OnLoadTrack -= SetStreamDisplayVariables;
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
			Color color = ApplicationManager.theme.GetColor("normal_font_color", Constants.THEME_TYPE);

			AddThemeColorOverride("font_color", color);
			AddThemeColorOverride("font_focus_color", color);
			AddThemeColorOverride("font_pressed_color", color);
			AddThemeColorOverride("font_hover_color", color);
			AddThemeColorOverride("font_pressed_color", color);
		}
	}

	public void SetStreamDisplayVariables()
	{
		display.cover_texture_rect.Texture = Globals.player.track_cover_texture_rect.Texture;
		display.track_name_label.Text = Globals.player.track_name_label.Text;
		display.artist_label.Text = Globals.player.track_artist_label.Text;
		display.background_texture_rect.Texture = texture.Texture;
    }
}
