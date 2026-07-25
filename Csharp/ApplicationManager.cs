using Godot;
using System;

public partial class ApplicationManager : Node
{
	static readonly string SOFTWARE_NAME = (string)ProjectSettings.GetSetting("application/config/name");
}
