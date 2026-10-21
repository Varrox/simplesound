using Godot;

public partial class ToolList : ContextMenuButton {
    [Export] public PackedScene streamer_display_template;
    [Export] public TextureRect background_texture_rect;

    private StreamDisplay _streamer_display;

    public const int STREAMER_DISPLAY = 0;

    public override void _Ready() {
        base._Ready();

        // Add streamer button

        GetPopup().AddCheckItem("Streamer Mode", STREAMER_DISPLAY);
    }

    protected override void _ItemPressed(long id) {
        base._ItemPressed(id);

        switch(id) {
            case STREAMER_DISPLAY:
                _ToggleStreamDisplay();
                break;
        }
    }

    private void _ToggleStreamDisplay() {
		if (IsStreamDisplayOpen()) { 
			_streamer_display = streamer_display_template.Instantiate() as StreamDisplay;
			GetTree().CurrentScene.AddChild(_streamer_display);

            _streamer_display.background_texture_rect.Texture = background_texture_rect.Texture;
        }
		else {
			_streamer_display.QueueFree();
		}
    }

    public void ToggleStreamDisplay() {
        _ItemPressed(STREAMER_DISPLAY);
    }

    public bool IsStreamDisplayOpen() {
        return GetPopup().IsItemChecked(STREAMER_DISPLAY);
    }
}
