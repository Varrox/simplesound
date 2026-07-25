using Godot;
using System;

[GlobalClass]
public partial class ThemeLineEdit : LineEdit
{
	public override void _Ready()
	{
        EditingToggled += ApplicationManager.OnTextEditingToggled;
	}
}
