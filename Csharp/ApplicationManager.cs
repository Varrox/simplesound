using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class ApplicationManager : SceneTree
{
	public static readonly string SOFTWARE_NAME = (string)ProjectSettings.GetSetting("application/config/name");

    private static readonly Vector2I _main_window_minimum_size = new Vector2I(850, 350);
    private static readonly Vector2I _main_window_default_size = new Vector2I((int)ProjectSettings.GetSetting("display/window/size/viewport_width"), (int)ProjectSettings.GetSetting("display/window/size/viewport_height"));

    public static readonly int reduce_fps_on_lose_focus_fps = 20;

    public static bool is_user_typing;

    private static int _last_windows_focused = 0;
    private static int _windows_focused = 0; 
    public static Window currently_focused_window;
    public static List<Window> windows = new List<Window>();
    private static List<Action> _focus_entered_actions = new List<Action>();
    private static List<Action> _focus_exited_actions = new List<Action>();

    public static Theme theme;

    private static ApplicationManager self;

    public ApplicationManager() {
        Globals.save_data = SaveData.GetSaveData();
        self = this;

        Root.MinSize = _main_window_minimum_size;
        Root.CloseRequested += QuitProgram;

        if (Globals.save_data.graphic_settings.main_display_size >= _main_window_minimum_size) { // If the size is not below minimum
            DisplayServer.WindowSetSize(Globals.save_data.graphic_settings.main_display_size);
        }
        else { // Set setting to default size if so, and save.
            Globals.save_data.graphic_settings.main_display_size = _main_window_default_size;
            Save();
        }

        theme = LoadTheme();

        AddWindow(Root);
        currently_focused_window = Root;
    }

    public override void _Initialize()
    {
        (CurrentScene as Control).Theme = theme;
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
        Globals.save_data.graphic_settings.main_display_size = self.Root.Size;
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

    private Theme LoadTheme() {
        Theme theme_resource = ResourceLoader.Load<Theme>("res://Styling/global_theme.tres");

        // Color

        theme_resource.SetColor("selected_font_color", Constants.THEME_TYPE, Colors.White);
        theme_resource.SetColor("unselected_font_color", Constants.THEME_TYPE, Colors.Gray);

        theme_resource.SetColor("normal_font_color", Constants.THEME_TYPE, Colors.White);
        theme_resource.SetColor("small_font_color", Constants.THEME_TYPE, Colors.DarkGray);

        theme_resource.SetColor("disabled_font_color", Constants.THEME_TYPE, Colors.DimGray);
        theme_resource.SetColor("enabled_font_color", Constants.THEME_TYPE, Colors.Lime);

        theme_resource.SetColor("playing_font_color", Constants.THEME_TYPE, Color.FromHtml("66ff5e"));

        theme_resource.SetColor("highlight_color", Constants.THEME_TYPE, Color.FromHtml("47474796"));

        // Constant

        // Font

        // Font size

        // Icon

        theme_resource.SetIcon("play_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/play.png"));
        theme_resource.SetIcon("pause_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/pause.png"));

        theme_resource.SetIcon("no_play_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/no_play.png"));

        theme_resource.SetIcon("loop_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/loop.png"));
        theme_resource.SetIcon("shuffle_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/shuffle.png"));

        theme_resource.SetIcon("mute_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/mute.png"));
        theme_resource.SetIcon("unmute_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/unmute.png"));

        theme_resource.SetIcon("forward_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/forward.png"));
        theme_resource.SetIcon("back_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/back.png"));

        theme_resource.SetIcon("forward_arrow_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/forward arrow.png"));
        theme_resource.SetIcon("back_arrow_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/back arrow.png"));

        theme_resource.SetIcon("playlist_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/playlist.png"));

        theme_resource.SetIcon("hamburger_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/list.png"));
        theme_resource.SetIcon("meatballs_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/more.png"));

        theme_resource.SetIcon("plus_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/plus.png"));
        theme_resource.SetIcon("x_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/x.png"));

        theme_resource.SetIcon("update_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/update.png"));
        theme_resource.SetIcon("refresh_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/refresh.png"));
        theme_resource.SetIcon("settings_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/settings.png"));

        theme_resource.SetIcon("folder_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/folder.png"));

        theme_resource.SetIcon("default_cover_icon", Constants.THEME_TYPE, GD.Load<Texture2D>("res://Icons/Default Covers/DefaultCover.png"));

        // Stylebox

        theme_resource.SetStylebox("color_picker_panel_stylebox", Constants.THEME_TYPE, GD.Load<StyleBox>("res://Styling/Color Picker/color_picker_panel.tres"));

        theme_resource.SetStylebox("radio_normal_stylebox", Constants.THEME_TYPE, GD.Load<StyleBox>("res://Styling/Selectables/radio_normal.tres"));
        theme_resource.SetStylebox("radio_hover_stylebox", Constants.THEME_TYPE, GD.Load<StyleBox>("res://Styling/Selectables/radio_hover.tres"));
        theme_resource.SetStylebox("radio_pressed_stylebox", Constants.THEME_TYPE, GD.Load<StyleBox>("res://Styling/Selectables/radio_pressed.tres"));
        theme_resource.SetStylebox("radio_disabled_stylebox", Constants.THEME_TYPE, GD.Load<StyleBox>("res://Styling/Selectables/radio_disabled.tres"));

        theme_resource.SetStylebox("radio_selected_normal_stylebox", Constants.THEME_TYPE, GD.Load<StyleBox>("res://Styling/Selectables/Selected/radio_selected_normal.tres"));
        theme_resource.SetStylebox("radio_selected_hover_stylebox", Constants.THEME_TYPE, GD.Load<StyleBox>("res://Styling/Selectables/Selected/radio_selected_hover.tres"));
        theme_resource.SetStylebox("radio_selected_pressed_stylebox", Constants.THEME_TYPE, GD.Load<StyleBox>("res://Styling/Selectables/Selected/radio_selected_pressed.tres"));
        theme_resource.SetStylebox("radio_selected_disabled_stylebox", Constants.THEME_TYPE, GD.Load<StyleBox>("res://Styling/Selectables/Selected/radio_selected_disabled.tres"));

        return theme_resource;
    }

    public static void SetRadioSelected(Control control, bool selected) {
        control.AddThemeStyleboxOverride("normal", theme.GetStylebox(selected ? "radio_selected_normal_stylebox" : "radio_normal_stylebox", Constants.THEME_TYPE));
        control.AddThemeStyleboxOverride("hover", theme.GetStylebox(selected ? "radio_selected_hover_stylebox" : "radio_hover_stylebox", Constants.THEME_TYPE));
        control.AddThemeStyleboxOverride("pressed", theme.GetStylebox(selected ? "radio_selected_pressed_stylebox" : "radio_pressed_stylebox", Constants.THEME_TYPE));
        control.AddThemeStyleboxOverride("disabled", theme.GetStylebox(selected ? "radio_selected_disabled_stylebox" : "radio_disabled_stylebox", Constants.THEME_TYPE));
    }
}
