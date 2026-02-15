using Godot;
using System;
using System.IO;

public partial class DownloadWindowOpener : EditorWindowOpener
{
    public override void _Ready()
    {
        window = Globals.download_window;
        base._Ready();
        ButtonUp += OpenSettings;
    }

    public void OpenSettings()
	{
		if (Globals.player.Interrupt())
		{
			(window as DownloadWindow).Open();
            ButtonPressed = false;
		}
	}
}