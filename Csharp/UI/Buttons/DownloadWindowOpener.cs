using Godot;

public partial class DownloadWindowOpener : Button {

    public override void _Ready() {
        ButtonUp += OpenDownloadWindow;
    }

    public void OpenDownloadWindow() {
        Main.instance.Pause();

        DownloadWindow download_window = (DownloadWindow)ApplicationManager.GetSceneResource("download_window").Instantiate();
        GetTree().Root.AddChid(download_window);

        download_window.Open();
		
        ButtonPressed = false;
	}
}