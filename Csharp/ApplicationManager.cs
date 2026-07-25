using Godot;
using System;

public partial class ApplicationManager : Node
{
	static readonly string SOFTWARE_NAME = (string)ProjectSettings.GetSetting("application/config/name");

    public static bool is_user_typing;

    public static void OnTextEditingToggled(bool toggled_on) {
        is_user_typing = toggled_on;
    }
}
