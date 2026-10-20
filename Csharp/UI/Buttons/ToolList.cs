using Godot;
using System;

public partial class ToolList : MenuButton
{
    [Export] public PackedScene streamer_display_template;
    [Export] public TextureRect background_texture_rect;

    private StreamDisplay _streamer_display;

    private const int STREAMER_DISPLAY = 0;

    public override void _Ready()
    {
        ApplicationManager.ApplyPopupMenuStyle(GetPopup());

        // Add streamer button

        GetPopup().AddCheckItem("Streamer Mode", STREAMER_DISPLAY);

        // Connections

        GetPopup().IdPressed += ItemPressed;
    }

    public void ItemPressed(long id)
    {
        if (GetPopup().IsItemCheckable((int)id))
        {
            GetPopup().SetItemChecked((int)id, !GetPopup().IsItemChecked((int)id));
            GetPopup().Show();
        }
        switch(id)
        {
            case STREAMER_DISPLAY:
                ToggleStreamDisplay();
                break;
        }
    }

    public void ToggleStreamDisplay() {
		if (GetPopup().IsItemChecked(STREAMER_DISPLAY)) { 
			_streamer_display = streamer_display_template.Instantiate() as StreamDisplay;
			GetTree().CurrentScene.AddChild(_streamer_display);

			Globals.main.OnLoadTrack += SetStreamDisplayVariables;
			SetStreamDisplayVariables();
        }
		else {
			_streamer_display.QueueFree();
			Globals.main.OnLoadTrack -= SetStreamDisplayVariables;
		}
	}

    public void SetStreamDisplayVariables() {
		_streamer_display.cover_texture_rect.Texture = Globals.player.track_cover_texture_rect.Texture;
		_streamer_display.track_name_label.Text = Globals.player.track_name_label.Text;
		_streamer_display.artist_label.Text = Globals.player.track_artist_label.Text;
		_streamer_display.background_texture_rect.Texture = background_texture_rect.Texture;
    }
}
