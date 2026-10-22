using Godot;

public partial class Appdata : Button {
	public override void _Ready() {
		Icon = ApplicationManager.GetThemeIcon("folder_icon");

		ButtonUp += () => OS.ShellShowInFileManager(Constants.USER_DATA);
	}
}
