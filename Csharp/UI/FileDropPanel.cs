using Godot;

public partial class FileDropPanel : Panel {
	[Export] public string file_filter;

	[Signal]
	public delegate void FilesDroppedEventHandler(string[] files);

	public override void _Ready() {
		GetWindow().FilesDropped += CheckFiles;

		TooltipText = "You can drag & drop files here";
		AddThemeStyleboxOverride("panel", ApplicationManager.GetThemeStylebox("file_drop_normal_stylebox"));
	}

	private bool IsMouseInRect() {
		return GetGlobalRect().HasPoint(GetWindow().GetMousePosition());
	}

	private void CheckFiles(string[] files) {
		if(IsMouseInRect()) {
			EmitSignal(SignalName.FilesDropped, files);
		}
	}
}
