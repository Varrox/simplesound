using Godot;
using System;
using System.Collections.Generic;

public partial class ApplicationManager : Node
{
	static readonly string SOFTWARE_NAME = (string)ProjectSettings.GetSetting("application/config/name");

    public static bool is_user_typing;

    public static Window currently_focused_window;
    public static List<Window> windows = new List<Window>();
    private static List<Action> _focus_entered_actions = new List<Action>();

    public override void _Ready()
    {
        AddWindow(GetTree().Root);
        currently_focused_window = GetTree().Root;
    }

    public static void OnTextEditingToggled(bool toggled_on) {
        is_user_typing = toggled_on;
    }

    public static void AddWindow(Window window) {
        if (windows.Contains(window)) {
            return;
        }

        windows.Add(window);
        _focus_entered_actions.Add(() => currently_focused_window = window);
        window.FocusEntered += _focus_entered_actions[_focus_entered_actions.Count - 1];
    }

    public static void RemoveWindow(Window window) {
        if (!windows.Contains(window)) {
            return;
        }

        int idx = windows.IndexOf(window);

        window.FocusEntered -= _focus_entered_actions[idx];
        _focus_entered_actions.RemoveAt(idx);
        windows.Remove(window);
    }
}
