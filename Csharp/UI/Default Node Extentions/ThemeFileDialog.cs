using Godot;

public partial class ThemeFileDialog : FileDialog {
	public override void _Ready() {
		UseNativeDialog = true;
		ShowHiddenFiles = true;

		Access = AccessEnum.Filesystem;
		InitialPosition = WindowInitialPosition.CenterPrimaryScreen;
	}

	public void SetTracks() {
		Filters = new[] { "*.mp3", "*.wav", "*.ogg" };
		FileMode = FileModeEnum.OpenFiles;
		OkButtonText = "Import audio files";
		Title = "Select Audio files";
	}

	public void SetFile() {
		Filters = null;
		FileMode = FileModeEnum.OpenFile;
		OkButtonText = "Select file";
		Title = "Select file";
	}

	public void SetCover() {
		Filters = new[] { "*.jpeg", "*.jpg", "*.png", "*.webp" };
		FileMode = FileModeEnum.OpenFile;
		OkButtonText = "Import Cover";
		Title = "Select Cover";
	}

	public void SetPlaylist() {
		Filters = new[] { "*.json"};
		FileMode = FileModeEnum.OpenFile;
		OkButtonText = "Import Playlist";
		Title = "Select Playlist";
	}
	
	public void SetZIP() {
		Filters = new[] { "*.zip"};
		FileMode = FileModeEnum.OpenFile;
		OkButtonText = "Import Zip Archive";
		Title = "Select Zip Archive";
	}
}
