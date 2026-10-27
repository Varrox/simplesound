using Godot;
using System;
using System.Collections.Generic;

[GlobalClass]
public partial class ApplicationManager : SceneTree {
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
    public static ResourceManager resource_manager;

    public static SaveData save_data;

    public static ApplicationManager instance;

    public ApplicationManager() {
        instance = this;

        save_data = SaveData.GetSaveData();

        theme = _LoadTheme();
        resource_manager = _LoadResourceManager();

        Root.MinSize = _main_window_minimum_size;
        Root.CloseRequested += QuitProgram;

        if (save_data.graphic_settings.main_display_size >= _main_window_minimum_size) { // If the size is not below minimum
            DisplayServer.WindowSetSize(save_data.graphic_settings.main_display_size);
        }
        else { // Set setting to default size if so, and save.
            save_data.graphic_settings.main_display_size = _main_window_default_size;
            Save();
        }

        AddWindow(Root);
        currently_focused_window = Root;
    }

    public override void _Initialize() {
        (CurrentScene as Control).Theme = theme;
    }

    public override bool _Process(double delta) {
        if (_last_windows_focused != _windows_focused) {
            _SetMaxFPS(_windows_focused != 0);
            _last_windows_focused = _windows_focused;
        }

        return false;
    }

    public static void Save() {
        save_data.Save();
    }

    public override void _Finalize() {
        Discord.ShutDown();
        Save();
    }

    public static void QuitProgram() {
        save_data.graphic_settings.main_display_size = instance.Root.Size;
        instance.Quit();
    }

    private static void _SetMaxFPS(bool focused) {
        Engine.MaxFps = focused || !save_data.graphic_settings.reduce_fps_on_lose_focus ? save_data.graphic_settings.max_fps : reduce_fps_on_lose_focus_fps;
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

    public static bool IsAnyWindowFocused() {
        for (int i = 0; i < windows.Count; i++) {
            if(windows[i].HasFocus()) {
                return true;
            }
        }

        return false;
    }

    private Theme _LoadTheme() {
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

        theme_resource.SetColor("icon_pressed_color", Constants.THEME_TYPE, Colors.DimGray);
        theme_resource.SetColor("icon_hover_color", Constants.THEME_TYPE, Colors.LightGray);

        // Constant

        theme_resource.SetConstant("context_menu_margin_constant", Constants.THEME_TYPE, 5);

        theme_resource.SetConstant("context_button_icon_max_width_small_constant", Constants.THEME_TYPE, 20);

        // Font

        // Font size

        // Icon

        theme_resource.SetIcon("play_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/play.svg"));
        theme_resource.SetIcon("pause_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/pause.svg"));
        theme_resource.SetIcon("stop_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/stop.svg"));

        theme_resource.SetIcon("no_play_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/no_play.svg"));

        theme_resource.SetIcon("loop_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/loop.svg"));
        theme_resource.SetIcon("shuffle_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/shuffle.svg"));

        theme_resource.SetIcon("mute_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/un_muted.svg"));
        theme_resource.SetIcon("unmute_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/muted.svg"));

        theme_resource.SetIcon("forward_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/forward.svg"));
        theme_resource.SetIcon("back_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/back.svg"));

        theme_resource.SetIcon("forward_arrow_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/forward_arrow.svg"));
        theme_resource.SetIcon("back_arrow_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/back_arrow.svg"));

        theme_resource.SetIcon("playlist_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/playlist.svg"));

        theme_resource.SetIcon("hamburger_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/hamburger.svg"));
        theme_resource.SetIcon("meatballs_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/meatballs.svg"));

        theme_resource.SetIcon("search_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/search.svg"));

        theme_resource.SetIcon("plus_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/plus.svg"));
        theme_resource.SetIcon("minus_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/minus.svg"));

        theme_resource.SetIcon("x_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/x.svg"));

        theme_resource.SetIcon("download_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/download.svg"));
        theme_resource.SetIcon("upload_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/upload.svg"));

        theme_resource.SetIcon("reload_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/reload.svg"));

        theme_resource.SetIcon("cog_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/cog.svg"));
        theme_resource.SetIcon("pen_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/pen.svg"));

        theme_resource.SetIcon("time_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/time.svg"));

        theme_resource.SetIcon("folder_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/folder.svg"));
        theme_resource.SetIcon("file_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/file.svg"));
        theme_resource.SetIcon("zip_icon", Constants.THEME_TYPE, GD.Load<DpiTexture>("res://Icons/SVG/zip.svg"));

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

        theme_resource.SetStylebox("file_drop_normal_stylebox", Constants.THEME_TYPE, GD.Load<StyleBox>("res://Styling/Filedrop/file_drop_normal.tres"));

        theme_resource.SetStylebox("context_menu_panel_stylebox", Constants.THEME_TYPE, GD.Load<StyleBox>("res://Styling/context_panel.tres"));
        
        return theme_resource;
    }

    private ResourceManager _LoadResourceManager() {
        ResourceManager resource_manager = new ResourceManager();

        resource_manager.SetResource("sound_visualizer", GD.Load<PackedScene>("res://Prefabs/sound_visualizer.tscn"));

        resource_manager.SetResource("playlist_display", GD.Load<PackedScene>("res://Prefabs/playlist_display.tscn"));
        resource_manager.SetResource("track_display", GD.Load<PackedScene>("res://Prefabs/track_display.tscn"));

        resource_manager.SetResource("confirmation_window", GD.Load<PackedScene>("res://Prefabs/confirmation_window.tscn"));

        resource_manager.SetResource("playlist_creator_window", GD.Load<PackedScene>("res://Windows/playlist_creator_window.tscn"));
        resource_manager.SetResource("metadata_editor_window", GD.Load<PackedScene>("res://attribute_editor_window.tscn"));
        
        resource_manager.SetResource("stream_display_window", GD.Load<PackedScene>("res://Windows/stream_display_window.tscn"));
        
        resource_manager.SetResource("download_window", GD.Load<PackedScene>("res://Windows/Downloading/download_window.tscn"));
        resource_manager.SetResource("settings_window", GD.Load<PackedScene>("res://Windows/settings_window.tscn"));

        return resource_manager;
    }

    public static Resource GetResource(StringName name) {
        if (resource_manager == null) return null;
        return resource_manager.GetResource(name);
    }

    public static PackedScene GetSceneResource(StringName name) {
        if (resource_manager == null) return null;
        return resource_manager.GetSceneResource(name);
    }

    public static Color GetThemeColor(StringName name) {
        if (theme == null) return Colors.White;
        return theme.GetColor(name, Constants.THEME_TYPE);
    }
    
    public static int GetThemeConstant(StringName name) {
        if (theme == null) return 0;
        return theme.GetConstant(name, Constants.THEME_TYPE);
    }

    public static Font GetThemeFont(StringName name) {
        if (theme == null) return null;
        return theme.GetFont(name, Constants.THEME_TYPE);
    }

    public static int GetThemeFontSize(StringName name) {
        if (theme == null) return 10;
        return theme.GetFontSize(name, Constants.THEME_TYPE);
    }

    public static Texture2D GetThemeIcon(StringName name) {
        if (theme == null) return null;
        return theme.GetIcon(name, Constants.THEME_TYPE);
    }

    public static StyleBox GetThemeStylebox(StringName name) {
        if (theme == null) return null;
        return theme.GetStylebox(name, Constants.THEME_TYPE);
    }

    public static void SetRadioSelected(Control control, bool selected) {
        control.AddThemeStyleboxOverride("normal", GetThemeStylebox(selected ? "radio_selected_normal_stylebox" : "radio_normal_stylebox"));
        control.AddThemeStyleboxOverride("hover", GetThemeStylebox(selected ? "radio_selected_hover_stylebox" : "radio_hover_stylebox"));
        control.AddThemeStyleboxOverride("pressed", GetThemeStylebox(selected ? "radio_selected_pressed_stylebox" : "radio_pressed_stylebox"));
        control.AddThemeStyleboxOverride("disabled", GetThemeStylebox(selected ? "radio_selected_disabled_stylebox" : "radio_disabled_stylebox"));
    }

    public static void ApplyPopupMenuStyle(PopupMenu menu)
    {
        menu.AddThemeStyleboxOverride("panel", GetThemeStylebox("context_menu_panel_stylebox"));
        
        int margin = GetThemeConstant("context_menu_margin_constant");

        menu.AddThemeConstantOverride("v_separation", margin);
        menu.AddThemeConstantOverride("h_separation", margin);
        menu.AddThemeConstantOverride("item_start_padding", margin);

        menu.AddThemeConstantOverride("icon_max_width", 22);
    }

    public static void CopySystemInfoToClipboard() {
		string operating_system = _GetOperatingSystem();
		string software_version = _GetSoftwareVersion();
		string gpu = _GetGPU();
		string cpu = _GetCPU();
		string memory = _GetMemory();
		
		string to_copy = string.Join(" - ", [operating_system, software_version, gpu, cpu, memory]);
		
		DisplayServer.ClipboardSet(to_copy);
	}

	private static string _GetOperatingSystem() {
		string str = OS.GetVersionAlias();
		return $"{OS.GetName()} {str.Substr(0, str.IndexOf(' '))}";
	}

	private static string _GetSoftwareVersion() {
		return $"{ApplicationManager.SOFTWARE_NAME} {ProjectSettings.GetSetting("application/config/version")}";
	}

	private static string _GetGPU() {
		return $"{RenderingServer.GetVideoAdapterName()} ({RenderingServer.GetVideoAdapterVendor()}; {OS.GetVideoAdapterDriverInfo()[1]})";
	}

	private static string _GetCPU() {
		return $"{OS.GetProcessorName()} ({OS.GetProcessorCount()} threads)";
	}

	private static string _GetMemory() {
		return $"{((double)OS.GetMemoryInfo()["physical"] / 1073741824.0).ToString("0.##")}GiB memory";
	}
}
