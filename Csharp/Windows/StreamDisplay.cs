using Godot;

public partial class StreamDisplay : Window {
	[Export] public TextureRect cover_texture_rect, background_texture_rect;
	[Export] public Label track_name_label, artist_label;

    public override void _Ready() {
        Main.instance.OnLoadTrack += _SetDisplayInfo;

		_SetDisplayInfo();
    }

    public override void _ExitTree() {
        Main.instance.OnLoadTrack -= _SetDisplayInfo;
    }

	private void _SetDisplayInfo() {
		cover_texture_rect.Texture = Player.instance.track_cover_texture_rect.Texture;
		track_name_label.Text = Player.instance.track_name_label.Text;
		artist_label.Text = Player.instance.track_artist_label.Text;
	}
}
