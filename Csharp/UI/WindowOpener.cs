using Godot;

public partial class EditorWindowOpener : Button
{
	[Export] public EditorWindow window;

	public override void _Ready()
	{
		window.Hide();
	}
}
