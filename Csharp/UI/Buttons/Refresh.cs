using Godot;

public partial class Refresh : Button {
	[Export] public AnimationPlayer refesh_texture_animation;
    [Export] public TextureRect refresh_texture_rect;

	public override void _Ready() {
        refresh_texture_rect.Texture = ApplicationManager.GetThemeIcon("reload_icon");

        ButtonUp += _Refresh;
    }

    public override void _Process(double delta) {
        if (Input.IsActionJustPressed("refresh")) {
            _Refresh();
        }
    }

    private void _Refresh() {
        refesh_texture_animation.Play("refresh");
        Globals.main.Refresh();
    }
}
