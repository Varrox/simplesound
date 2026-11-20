using Godot;

public partial class SettingsOpener : Button {

    public override void _Ready() {
        ButtonUp += OpenSettingsWindow;
    }

    public void OpenSettingsWindow() {
        Main.instance.Pause();

        DownloadWindow settings_window = (DownloadWindow)ApplicationManager.GetSceneResource("settings_window").Instantiate();
        GetTree().Root.AddChid(settings_window);

        settings_window.Open();
		
        ButtonPressed = false;
	}
}