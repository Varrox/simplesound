using Godot;

public partial class Appdata : Button
{
	public override void _Ready()
	{
		ButtonUp += () => OS.ShellShowInFileManager(Constants.USER_DATA);
	}
}
