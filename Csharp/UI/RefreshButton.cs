using Godot;

public partial class RefreshButton : Button
{
	public override void _Ready()
	{
		ButtonDown += Globals.main.Refresh;
	}
}
