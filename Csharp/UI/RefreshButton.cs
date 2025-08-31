using Godot;

public partial class RefreshButton : Button
{
	[Export] public AnimationPlayer refesh_texture_animation;
	public override void _Ready()
	{
        ButtonUp += () => refesh_texture_animation.Play("refresh");
        ButtonUp += Globals.main.Refresh;
    }

    public override void _Process(double delta)
    {
        if (Input.IsActionJustPressed("refresh"))
        {
            refesh_texture_animation.Play("refresh");
            Globals.main.Refresh();
        }
    }
}
