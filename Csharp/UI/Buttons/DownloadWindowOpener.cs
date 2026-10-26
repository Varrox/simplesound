using Godot;

public partial class DownloadWindowOpener : EditorWindowOpener {
    [Export] public PackedScene download_window_template;

    public override void _Ready() {
        DownloadWindow download_window = (DownloadWindow)download_window_template.Instantiate();
        GetTree().Root.CallDeferred("add_child", download_window);

        download_window.Hide();

        window = download_window;
        
        base._Ready();
        ButtonUp += OpenSettings;
    }

    public void OpenSettings() {
        Main.instance.Pause();
		
        (window as DownloadWindow).Open();
        ButtonPressed = false;
	}
}