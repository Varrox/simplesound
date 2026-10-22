using Godot;

public partial class Appdata : Button {
	public override void _Ready() {
		Icon = ApplicationManager.theme.GetIcon("folder_icon", Constants.THEME_TYPE);

		ButtonUp += () => OS.ShellShowInFileManager(Constants.USER_DATA);
	}
}
