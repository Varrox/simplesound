using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class ApplicationManager : SceneTree
{
	public static readonly string SOFTWARE_NAME = (string)ProjectSettings.GetSetting("application/config/name");

    private static readonly Vector2I _main_window_minimum_size = new Vector2I(850, 350);

    public static readonly int reduce_fps_on_lose_focus_fps = 20;

    public static bool is_user_typing;

    private static int _last_windows_focused = 0;
    private static int _windows_focused = 0; 
    public static Window currently_focused_window;
    public static List<Window> windows = new List<Window>();
    private static List<Action> _focus_entered_actions = new List<Action>();
    private static List<Action> _focus_exited_actions = new List<Action>();

    private static ApplicationManager self;

    public ApplicationManager() {
        Globals.save_data = SaveData.GetSaveData();
        self = this;

        Root.MinSize = _main_window_minimum_size;

        AddWindow(Root);
        currently_focused_window = Root;
    }

    public override bool _Process(double delta)
    {
        if (_last_windows_focused != _windows_focused) {
            _SetMaxFPS(_windows_focused != 0);
            _last_windows_focused = _windows_focused;
        }

        return false;
    }

    public static void Save() {
        Globals.save_data.Save();
    }

    public override void _Finalize() {
        Discord.ShutDown();
        Save();
    }

    public static void QuitProgram() {
        Globals.save_data.graphic_settings.main_display_size = (SerialVector2I)self.Root.Size;
        self.Quit();
    }

    private static void _SetMaxFPS(bool focused) {
        Engine.MaxFps = focused || !Globals.save_data.graphic_settings.reduce_fps_on_lose_focus ? Globals.save_data.graphic_settings.max_fps : reduce_fps_on_lose_focus_fps;
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
