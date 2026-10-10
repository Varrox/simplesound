using Godot;
using System;

public partial class FileDropPanel : Panel {
	[Export] public string file_filter;

	public Action<string[]> FilesDropped;

	public override void _Ready() {
		GetWindow().FilesDropped += CheckFiles;

		TooltipText = "You can drag & drop files here";
		AddThemeStyleboxOverride("panel", ApplicationManager.theme.GetStylebox("file_drop_normal_stylebox", Constants.THEME_TYPE));
	}

	private bool IsMouseInRect() {
		return GetGlobalRect().HasPoint(GetWindow().GetMousePosition());
	}

	private void CheckFiles(string[] files) {
		if(IsMouseInRect()) {
			FilesDropped?.Invoke(files);
		}
	}
}
