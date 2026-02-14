using Godot;
using System;

[GlobalClass]
public partial class SettingsTab : Resource
{
    [Export] public string section_name;
    [Export] public string[] settings;
    [Export] public bool disabled = false;
}
