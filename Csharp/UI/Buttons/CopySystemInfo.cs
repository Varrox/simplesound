using Godot;

public partial class CopySystemInfo : Button {
	public override void _Ready() {
		ButtonUp += ApplicationManager.CopySystemInfoToClipboard;
	}

	
}
