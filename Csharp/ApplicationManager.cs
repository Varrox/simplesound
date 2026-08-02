using Godot;
using System;
using System.Collections.Generic;

public partial class ApplicationManager : Node
{
	static readonly string SOFTWARE_NAME = (string)ProjectSettings.GetSetting("application/config/name");

    public static bool is_user_typing;

    private static int _last_windows_focused = 0;
    private static int _windows_focused = 0; 
    public static Window currently_focused_window;
    public static List<Window> windows = new List<Window>();
    private static List<Action> _focus_entered_actions = new List<Action>();
    private static List<Action> _focus_exited_actions = new List<Action>();

    public override void _Ready()
    {
        AddWindow(GetTree().Root);
        currently_focused_window = GetTree().Root;
    }

    public override void _Process(double delta)
    {
        if (_last_windows_focused != _windows_focused) {
            _SetMaxFPS(_windows_focused != 0);
            _last_windows_focused = _windows_focused;
        }
    }

    private static void _SetMaxFPS(bool focused) {
        Engine.MaxFps = focused || !Globals.save_data.graphic_settings.reduce_fps_on_lose_focus ? Globals.save_data.graphic_settings.max_fps : 20;
    }

    public static void OnTextEditingToggled(bool toggled_on) {
        is_user_typing = toggled_on;
    }

    public static void AddWindow(Window window) {
        if (windows.Contains(window)) {
            return;
        }

        windows.Add(window);

        _focus_entered_actions.Add(() => { currently_focused_window = window; _windows_focused++; });
        window.FocusEntered += _focus_entered_actions[_focus_entered_actions.Count - 1];

        _focus_exited_actions.Add(() => _windows_focused--);
        window.FocusExited += _focus_exited_actions[_focus_exited_actions.Count - 1];
    }

    public static void RemoveWindow(Window window) {
        if (!windows.Contains(window)) {
            return;
        }

        int idx = windows.IndexOf(window);

        window.FocusEntered -= _focus_entered_actions[idx];
        _focus_entered_actions.RemoveAt(idx);

        window.FocusExited -= _focus_exited_actions[idx];
        _focus_exited_actions.RemoveAt(idx);

        windows.Remove(window);
    }
}
