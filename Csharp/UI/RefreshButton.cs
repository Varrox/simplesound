using Godot;

public partial class RefreshButton : Button
{
	[Export] public Main main;
	public override void _Ready()
	{
		ButtonDown += main.Refresh;
	}
}
