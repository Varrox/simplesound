public partial class DownloadWindowOpener : EditorWindowOpener {
    public override void _Ready() {
        window = Globals.download_window;
        base._Ready();
        ButtonUp += OpenSettings;
    }

    public void OpenSettings() {
        Main.instance.Pause();
		
        (window as DownloadWindow).Open();
        ButtonPressed = false;
	}
}