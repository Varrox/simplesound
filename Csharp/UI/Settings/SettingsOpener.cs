using Godot;
using System;

public partial class SettingsOpener : EditorWindowOpener
{
    public override void _Ready()
    {
        base._Ready();
        ButtonUp += OpenSettings;
    }

    public void OpenSettings()
	{
		if (Globals.player.Interrupt())
		{
			(window as Settings).Show();
            ButtonPressed = false;
		}
	}
}
