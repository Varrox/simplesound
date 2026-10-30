using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

public partial class Settings : EditorWindow {
    [Export] public VBoxContainer settings_container;
    [Export] public VBoxContainer tabs_container;

    [Export] public Button close_menu;

    private List<SettingsTab> _settings_tabs = new List<SettingsTab>();
    private int _selected_tab;

    private const int SETTINGS_TAB_SIZE_X = 150, SETTINGS_TAB_SIZE_Y = 20;

    private const int LABEL_PADDING = 15;

    private struct SettingsTab {
        public Action add_settings;
        public Button button;
        public Label label;
        public HSeparator underline;
        public bool disabled;

        public SettingsTab(Action add_settings, Button button, Label label, HSeparator underline, bool disabled) {
            this.add_settings = add_settings;
            this.button = button;
            this.label = label;
            this.underline = underline;
            this.disabled = disabled;
        }
    }

    public override void _Ready() {
        base._Ready();

        AddSettingsTab("Audio", AddAudioSettings, false, "EQ, Reverb, etc.");
        AddSettingsTab("Downloader", AddDownloaderSettings, false, "yt-dlp executable, download formats, etc.");
        AddSettingsTab("App data", AddAppDataSettings, true, "");
        AddSettingsTab("Graphics / Display", AddGraphicSettings, false, "VSync, background blur quality, etc.");

        close_menu.ButtonUp += Close;
    }

    void AddAudioSettings() {
        AddHeader("EQ", """
        The "equalizer" (or EQ) gives you control over the gain of frequencies in the entire spectrum, by allowing their adjustment through bands. 
        A band is a point in the frequency spectrum, and each band means a division of the spectrum that can be adjusted.
        """);

        AddFloatSetting("Hz 32", ApplicationManager.save_data.audio_settings, "hz_32", 0.0f, -60.0f, 24.0f, "The 32 Hz band.");
        AddFloatSetting("Hz 100", ApplicationManager.save_data.audio_settings, "hz_100", 0.0f, -60.0f, 24.0f, "The 100 Hz band.");
        AddFloatSetting("Hz 320", ApplicationManager.save_data.audio_settings, "hz_320", 0.0f, -60.0f, 24.0f, "The 320 Hz band.");
        AddFloatSetting("Hz 1000", ApplicationManager.save_data.audio_settings, "hz_1000", 0.0f, -60.0f, 24.0f, "The 1900 Hz band.");
        AddFloatSetting("Hz 3200", ApplicationManager.save_data.audio_settings, "hz_3200", 0.0f, -60.0f, 24.0f, "The 3200 Hz band.");
        AddFloatSetting("Hz 10000", ApplicationManager.save_data.audio_settings, "hz_10000", 0.0f, -60.0f, 24.0f, "The 10000 Hz band.");

        AddHeader("Reverb", """
        The reverb effect plays the track audio back continuously, decaying over a period of time.
        It simulates sounds in different kinds of spaces, ranging from small rooms, to big caverns.
        """);

        AddFloatSetting("Room Size", ApplicationManager.save_data.audio_settings, "room_size", 0.0f, 0.0f, 1.0f, 
        "Dimensions of simulated room. Bigger means more echoes.");

        AddFloatSetting("Damping", ApplicationManager.save_data.audio_settings, "damping", 0.5f, 0.0f, 1.0f, 
        "Defines how reflective the imaginary room's walls are. The more reflective, the more high frequency content the reverb has.");

        AddFloatSetting("Spread", ApplicationManager.save_data.audio_settings, "spread", 1.0f, 0.0f, 1.0f,
        "Widens or narrows the stereo image of the reverb tail. At 1, it fully widens.");

        AddFloatSetting("High-Pass", ApplicationManager.save_data.audio_settings, "high_pass", 0.0f, 0.0f, 1.0f,
        "High-pass filter allows frequencies higher than a certain cutoff threshold and attenuates frequencies lower than the cutoff threshold.");

        AddFloatSetting("Dry", ApplicationManager.save_data.audio_settings, "dry", 1.0f, 0.0f, 1.0f, 
        "The volume ratio of the original audio. At 0, only the modified audio is outputted.");

        AddFloatSetting("Wet", ApplicationManager.save_data.audio_settings, "wet", 0.0f, 0.0f, 1.0f,
        "The volume ratio of the modified audio. At 0, only the original audio is outputted.");
    }

    void AddDownloaderSettings() {
        AddFileLocationSetting("yt-dlp Location", ApplicationManager.save_data.application_settings, "ytdlp_location", 
        "The file location of an yt-dlp executable.");
        AddEnumSetting("Audio Format", ApplicationManager.save_data.application_settings, "download_format", DownloadFormat.MP3, false, 
        "The audio format that the track downloader will save track as.");
    }

    void AddAppDataSettings() {
    }

    void AddGraphicSettings() {
        AddEnumSetting("Blur Quality", ApplicationManager.save_data.graphic_settings, "blur_quality", QualityLevel.MEDIUM, true, 
        "The quality of the blur effect on the background (Higher qualities will lower performance).");

        AddHeader("Display");

        AddBoolSetting("VSync", ApplicationManager.save_data.graphic_settings, "vsync", true, 
        "VSync changes your top FPS to be the same as your monitor refreshrate (regardless of Max FPS), saving power and CPU / GPU usage.");

        AddIntSetting("Max FPS", ApplicationManager.save_data.graphic_settings, "max_fps", 60, 0, 500, 
        $"The Maximum FPS that {ApplicationManager.SOFTWARE_NAME} can run at.");

        AddBoolSetting("Reduce FPS on lose focus", ApplicationManager.save_data.graphic_settings, "reduce_fps_on_lose_focus", true, $"""
        An option to reduce FPS to a maximum of {ApplicationManager.reduce_fps_on_lose_focus_fps} when the {ApplicationManager.SOFTWARE_NAME} window(s) is not focused to lower CPU / GPU usage.
        Note: Having this enabled while streamer mode is on isn't suggested as it will lower the FPS of the streamer window too (it affects all windows apart of {ApplicationManager.SOFTWARE_NAME}).
        """);
    }

    public void SelectSettingsTab(int tab) {
        if (tab >= 0 && tab < _settings_tabs.Count) {
            if (settings_container.GetChildCount() > 0)
                ClearSettings();
            
            _settings_tabs[tab].add_settings();

            // Highlight

            for(int i = 0; i < _settings_tabs.Count; i++) {
                if(_settings_tabs[i].disabled) continue;
                bool is_tab = i == tab;

                _settings_tabs[i].label.AddThemeColorOverride("font_color", ApplicationManager.GetThemeColor(is_tab ? "selected_font_color" : "unselected_font_color"));
                _settings_tabs[i].button.SelfModulate = is_tab ? ApplicationManager.GetThemeColor("highlight_color") : Colors.White;
                _settings_tabs[i].underline.Visible = is_tab;

            }
        }
        else GD.PrintErr($"Unable to select settings tab \'{tab}\'");
    }

    public void ClearSettings() {
        for (int i = 0; i < settings_container.GetChildren().Count; i++) {
            settings_container.GetChild(i).QueueFree();
        }
    }

    public void AddSettingsTab(string section_name, Action add_settings, bool disabled = false, string tooltip_text = "") {
        Vector2 button_size = new Vector2(SETTINGS_TAB_SIZE_X + LABEL_PADDING, SETTINGS_TAB_SIZE_Y + LABEL_PADDING);
        
        Button button = new Button
        {
            TooltipText = disabled ? "Disabled" : tooltip_text,
            Size = button_size,
            CustomMinimumSize = button_size,
            Disabled = disabled
        };

        tabs_container.AddChild(button);

        Label label = new Label {
            Text = section_name,
            VerticalAlignment = VerticalAlignment.Center,
            HorizontalAlignment = HorizontalAlignment.Center,
            Size = button_size,
            Position = Vector2.Zero,
            CustomMaximumSize = button_size,
            MouseFilter = Control.MouseFilterEnum.Ignore,
        };

        button.AddChild(label);
        
        label.SetAnchorsPreset(Control.LayoutPreset.FullRect);
        label.AddThemeColorOverride("font_color", ApplicationManager.GetThemeColor(disabled ? "disabled_font_color" : "unselected_font_color"));

        HSeparator underline = new HSeparator {
            Size = button_size,
            Position = Vector2.Zero,
            CustomMaximumSize = button_size,
            Visible = false,
            SelfModulate = new Color(1f, 1f, 1f, 0.25f),
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        label.AddChild(underline);

        underline.SetAnchorsPreset(Control.LayoutPreset.FullRect);

        underline.Size = new Vector2(button_size.X * (3.75f/4f), button_size.Y);
        underline.Position = new Vector2(button_size.X * (.25f/8f), button_size.Y * (.675f/2f));

        int tab_index = _settings_tabs.Count;

        button.ButtonUp += () => SelectSettingsTab(tab_index);

        _settings_tabs.Add(new SettingsTab(add_settings, button, label, underline, disabled));
    }

    public void AddHeader(string header_title, string tooltip_text = "") {
        HBoxContainer container = new HBoxContainer {
            CustomMinimumSize = Vector2.Down * 25,
            TooltipText = tooltip_text
        };

        settings_container.AddChild(container);

        HSeparator l_h_separator = new HSeparator {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        container.AddChild(l_h_separator);

        Label label = new Label {
            Text = header_title
        };

        container.AddChild(label);

        HSeparator r_h_separator = new HSeparator() {
            SizeFlagsHorizontal = Control.SizeFlags.ExpandFill,
            MouseFilter = Control.MouseFilterEnum.Ignore
        };

        container.AddChild(r_h_separator);
    }

    public void AddIntSetting<T>(string full_name, T where, string instance_name, int default_value, int min_value = 0, int max_value = int.MaxValue, string tooltip_text = "") where T : ISettings {
        Type type = typeof(T);

        IntegerInputField input_field = new IntegerInputField {
            full_name = full_name,
            TooltipText = tooltip_text,
            value = GetSetting<int>(where, type, instance_name),
            default_value = default_value,
            minimum_value = min_value,
            maximum_value = max_value
        };

        settings_container.AddChild(input_field);

        input_field.ValueChanged += (int new_value) => ApplySetting(new_value, where, type, instance_name);
    }

    public void AddFloatSetting<T>(string full_name, T where, string instance_name, float default_value, float min_value = 0.0f, float max_value = float.PositiveInfinity, string tooltip_text = "") where T : ISettings {
        Type type = typeof(T);

        FloatInputField input_field = new FloatInputField {
            full_name = full_name,
            TooltipText = $"{tooltip_text} Value can range from {min_value} to {max_value}.",
            value = GetSetting<float>(where, type, instance_name),
            default_value = default_value,
            minimum_value = min_value,
            maximum_value = max_value
        };

        settings_container.AddChild(input_field);

        input_field.ValueChanged += (float new_value) => ApplySetting(new_value, where, type, instance_name);
    }

    public void AddEnumSetting<T>(string full_name, T where, string instance_name, int default_value, string[] enum_values, string tooltip_text = "") where T : ISettings {
        Type type = typeof(T);

        EnumInputField input_field = new EnumInputField {
            full_name = full_name,
            TooltipText = tooltip_text,
            value = GetSetting<int>(where, type, instance_name),
            default_value = default_value,
            options = enum_values
        };

        settings_container.AddChild(input_field);

        input_field.ValueChanged += (int new_value) => ApplySetting(new_value, where, type, instance_name);
    }

    public void AddEnumSetting<T, E>(string full_name, T where, string instance_name, E default_value, bool capitalize_values = true, string tooltip_text = "") where T : ISettings where E : struct, Enum {
        Type type = typeof(T);

        EnumInputField input_field = EnumInputField.CreateFromEnum(
            GetSetting<E>(where, type, instance_name), // Value
            default_value, // Default Value
            capitalize_values // Capitalize
        );
        
        input_field.full_name = full_name;
        input_field.TooltipText = tooltip_text;

        settings_container.AddChild(input_field);

        input_field.ValueChanged += (int new_value) => ApplySetting((E)Enum.ToObject(typeof(E), new_value), where, type, instance_name);
    }

    public void AddBoolSetting<T>(string full_name, T where, string instance_name, bool default_value, string tooltip_text = "") where T : ISettings {
        Type type = typeof(T);

        BooleanInputField input_field = new BooleanInputField {
            full_name = full_name,
            TooltipText = tooltip_text,
            value = GetSetting<bool>(where, type, instance_name),
            default_value = default_value
        };

        settings_container.AddChild(input_field);

        input_field.ValueChanged += (bool new_value) => ApplySetting(new_value, where, type, instance_name);
    }

    public void AddStringSetting<T>(string full_name, T where, string instance_name, string default_value, string tooltip_text = "") where T : ISettings {
        Type type = typeof(T);

        StringInputField input_field = new StringInputField {
            full_name = full_name,
            TooltipText = tooltip_text,
            value = GetSetting<string>(where, type, instance_name),
            default_value = default_value
        };

        settings_container.AddChild(input_field);

        input_field.ValueChanged += (string new_value) => ApplySetting(new_value, where, type, instance_name);
    }

    public void AddFileLocationSetting<T>(string full_name, T where, string instance_name, string tooltip_text = "") where T : ISettings {
        Type type = typeof(T);

        FileInputField input_field = new FileInputField {
            full_name = full_name,
            TooltipText = tooltip_text,
            value = GetSetting<string>(where, type, instance_name)
        };

        settings_container.AddChild(input_field);

        input_field.ValueChanged += (string new_value) => ApplySetting(new_value, where, type, instance_name);
    }

    public void AddVector2Setting<T>(string full_name, T where, string instance_name, Vector2 default_value, Vector2 min_value, Vector2 max_value, string tooltip_text = "") where T : ISettings {
        Type type = typeof(T);

        Vector2InputField input_field = new Vector2InputField {
            full_name = full_name,
            TooltipText = tooltip_text,
            value = GetSetting<Vector2>(where, type, instance_name),
            default_value = default_value,
            minimum_value = min_value,
            maximum_value = max_value
        };

        settings_container.AddChild(input_field);

        input_field.ValueChanged += (Vector2 new_value) => ApplySetting(new_value, where, type, instance_name);
    }

    public void AddColorSetting<T>(string full_name, T where, string instance_name, Color default_value, string tooltip_text = "") where T : ISettings {
        Type type = typeof(T);

        ColorInputField input_field = new ColorInputField {
            full_name = full_name,
            TooltipText = tooltip_text,
            value = Color.FromString(GetSetting<string>(where, type, instance_name) ?? "", default_value),
            default_value = default_value
        };

        settings_container.AddChild(input_field);

        input_field.ValueChanged += (Color new_value) => ApplySetting(new_value, where, type, instance_name);
    }

    public void Open() {
        Show();
        SelectSettingsTab(0);
    }

    public void Close() {
        Hide();
    }

    T GetSetting<T>(ISettings where, Type type, string name) {
        return (T)(type.GetField(name).GetValue(where));
    }

    public void ApplySetting<T>(T value, ISettings where, Type type, string instance_name) {
        if (ApplicationManager.save_data == null) return;

        FieldInfo field = type.GetField(instance_name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (field != null) {
            field.SetValue(where, Convert.ChangeType(value, field.FieldType));

            ApplicationManager.save_data.Save();
            where.ApplySettings();
        }
        else GD.PrintErr($"Settings Field not found. Unable to set setting \'{instance_name}\'");
    }
}
