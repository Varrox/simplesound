using System;
using System.Collections.Generic;
using System.Reflection;
using Godot;

public partial class Settings : EditorWindow
{
    [Export] public VBoxContainer settings_container;
    [Export] public VBoxContainer tabs_container;
    [Export] public Texture2D reset_texture, file_texture;

    [Export] public Button close_menu;

    private List<SettingsTab> _settings_tabs = new List<SettingsTab>();
    private int _selected_tab;

    private const int SETTINGS_TAB_SIZE_X = 150, SETTINGS_TAB_SIZE_Y = 20;

    private const int LABEL_PADDING = 15;

    private struct SettingsTab {
        public Action add_settings;
        public Button button;
        public Label label;
        public bool disabled;

        public SettingsTab(Action add_settings, Button button, Label label, bool disabled) {
            this.add_settings = add_settings;
            this.button = button;
            this.label = label;
            this.disabled = disabled;
        }
    }

    public override void _Ready() {
        base._Ready();

        AddSettingsTab("Audio", AddAudioSettings);
        AddSettingsTab("Cloud", AddCloudSettings);
        AddSettingsTab("Downloader", AddDownloaderSettings);
        AddSettingsTab("App data", AddAppDataSettings, true);
        AddSettingsTab("Graphics / Display", AddGraphicSettings);

        close_menu.ButtonUp += Close;
    }

    public void SelectSettingsTab(int tab) {
        if (tab >= 0 && tab < _settings_tabs.Count) {
            if (settings_container.GetChildCount() > 0)
                ClearSettings();
            
            _settings_tabs[tab].add_settings();

            // Highlight

            _settings_tabs[tab].button.SelfModulate = Globals.highlight;
            _settings_tabs[tab].label.AddThemeColorOverride("font_color", Globals.text_highlight);

            for(int i = 0; i < _settings_tabs.Count; i++) {
                if (i != tab && !_settings_tabs[i].disabled) {
                    _settings_tabs[i].button.SelfModulate = Colors.White;
                    _settings_tabs[i].label.AddThemeColorOverride("font_color", Colors.White);
                }
            }
        }
        else GD.PrintErr($"Unable to select settings tab \'{tab}\'");
    }

    public void ClearSettings() {
        for (int i = 0; i < settings_container.GetChildren().Count; i++) {
            settings_container.GetChild(i).QueueFree();
        }
    }

    public void ClearResetButton(HBoxContainer container) { // Delete Reset button(s)
        foreach (Node node in container.GetChildren()) {
            if (node is Button && node.HasMeta("reset")) {
                node.QueueFree();
            }
        }
    }

    public void AddResetButton(HBoxContainer container, Action reset_value, Action apply_setting) {
        Button button = new Button();
        button.Icon = reset_texture;
        button.ExpandIcon = true;

        container.AddChild(button);
        button.SetMeta("reset", true);
        button.CustomMinimumSize = Vector2.One * 28;
        button.ButtonUp += () => { reset_value(); button.QueueFree(); apply_setting(); };
    }

    public HBoxContainer CreateSettingsItem(string full_name) {
        HBoxContainer container = new HBoxContainer();
        settings_container.AddChild(container);

        Label label = new Label();
        label.Text = full_name + ":";
        container.AddChild(label);

        return container;
    }

    public void AddSettingsTab(string section_name, Action add_settings, bool disabled = false) {
        Button button = new Button();

        tabs_container.AddChild(button);
        
        button.CustomMinimumSize = new Vector2(SETTINGS_TAB_SIZE_X + LABEL_PADDING, SETTINGS_TAB_SIZE_Y + LABEL_PADDING);

        button.Disabled = disabled;

        Label label = new Label();
        label.Text = section_name;

        button.AddChild(label);

        label.Size = button.CustomMinimumSize;
        label.Position = Vector2.Zero;
        label.CustomMaximumSize = button.CustomMinimumSize;
        label.SetAnchorsPreset(Control.LayoutPreset.FullRect);

        label.VerticalAlignment = VerticalAlignment.Center;
        label.HorizontalAlignment = HorizontalAlignment.Center;

        if (disabled) {
            button.TooltipText = "Disabled";
            label.AddThemeColorOverride("font_color", Globals.disabled_text_highlight);
        }

        int tab_index = _settings_tabs.Count;

        button.ButtonUp += () => SelectSettingsTab(tab_index);

        _settings_tabs.Add(new SettingsTab(add_settings, button, label, disabled));
    }

    void AddAudioSettings() {
        AddHeader("EQ");
        AddFloatSetting("HZ 32", Globals.save_data.audio_settings, "hz_32", 0.0f, -60.0f, 24.0f);
        AddFloatSetting("HZ 100", Globals.save_data.audio_settings, "hz_100", 0.0f, -60.0f, 24.0f);
        AddFloatSetting("HZ 320", Globals.save_data.audio_settings, "hz_320", 0.0f, -60.0f, 24.0f);
        AddFloatSetting("HZ 1000", Globals.save_data.audio_settings, "hz_1000", 0.0f, -60.0f, 24.0f);
        AddFloatSetting("HZ 3200", Globals.save_data.audio_settings, "hz_3200", 0.0f, -60.0f, 24.0f);
        AddFloatSetting("HZ 10000", Globals.save_data.audio_settings, "hz_10000", 0.0f, -60.0f, 24.0f);

        AddHeader("Reverb");
        AddFloatSetting("Room Size", Globals.save_data.audio_settings, "room_size", 0.0f, 0.0f, 1.0f);
        AddFloatSetting("Damping", Globals.save_data.audio_settings, "damping", 0.5f, 0.0f, 1.0f);
        AddFloatSetting("Spread", Globals.save_data.audio_settings, "spread", 1.0f, 0.0f, 1.0f);
        AddFloatSetting("High-Pass", Globals.save_data.audio_settings, "high_pass", 0.0f, 0.0f, 1.0f);
        AddFloatSetting("Dry", Globals.save_data.audio_settings, "dry", 1.0f, 0.0f, 1.0f);
        AddFloatSetting("Wet", Globals.save_data.audio_settings, "wet", 0.0f, 0.0f, 1.0f);
    }

    void AddCloudSettings() {
        AddBoolSetting("Sync Application Settings", Globals.save_data.cloud_settings, "sync_application_settings", true);
    }

    void AddDownloaderSettings() {
        AddFileLocationSetting("yt-dlp Location", Globals.save_data.application_settings, "ytdlp_location");
        AddEnumSetting("Audio Format", Globals.save_data.application_settings, "download_format", 0, Constants.DOWNLOAD_FORMATS);
    }

    void AddAppDataSettings() {
    }

    void AddGraphicSettings() {
        AddEnumSetting("Blur Quality", Globals.save_data.graphic_settings, "blur_quality", 2, Constants.QUALITY_LEVELS);
    }

    public void AddHeader(string header_title) {
        HBoxContainer container = new HBoxContainer();
        settings_container.AddChild(container);

        container.CustomMinimumSize = Vector2.Down * 25;

        HSeparator l_h_separator = new HSeparator();
        l_h_separator.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

        Label label = new Label();
        label.Text = header_title;

        HSeparator r_h_separator = new HSeparator();
        r_h_separator.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

        container.AddChild(l_h_separator);
        container.AddChild(label);
        container.AddChild(r_h_separator);
    }

    public void AddIntSetting<T>(string full_name, T where, string instance_name, int default_value, int min_value = 0, int max_value = int.MaxValue) where T : ISettings {
        Type type = typeof(T);

        HBoxContainer container = CreateSettingsItem(full_name);

        SpinBox spin_box = new SpinBox();
        int value = GetSetting<int>(where, type, instance_name);
        spin_box.Value = value;

        spin_box.MinValue = min_value;
        spin_box.MaxValue = max_value;

        void ApplyInt() { ApplySetting(spin_box.Value, where, type, instance_name); }
        void ResetInt() { spin_box.Value = default_value; }

        spin_box.ValueChanged += (double new_value) => ValueChanged(container, new_value, default_value, ResetInt, ApplyInt);

        container.AddChild(spin_box);

        if (value != default_value) 
            AddResetButton(container, ResetInt, ApplyInt);
    }

    public void AddFloatSetting<T>(string full_name, T where, string instance_name, float default_value, float min_value = 0.0f, float max_value = float.PositiveInfinity) where T : ISettings {
        Type type = typeof(T);

        HBoxContainer container = CreateSettingsItem(full_name);

        SpinBox spin_box = new SpinBox();
        float value = GetSetting<float>(where, type, instance_name);
        spin_box.Step = 0.01;
        spin_box.Value = value;

        spin_box.MinValue = min_value;
        spin_box.MaxValue = max_value;

        void ApplyFloat() { ApplySetting(spin_box.Value, where, type, instance_name); }
        void ResetFloat() { spin_box.Value = default_value; }

        spin_box.ValueChanged += (double new_value) => ValueChanged(container, new_value, default_value, ResetFloat, ApplyFloat);

        container.AddChild(spin_box);

        if (value != default_value) 
            AddResetButton(container, ResetFloat, ApplyFloat);
    }

    public void AddEnumSetting<T>(string full_name, T where, string instance_name, int default_value, string[] enum_values) where T : ISettings {
        Type type = typeof(T);
        
        HBoxContainer container = CreateSettingsItem(full_name);

        OptionButton option_button = new OptionButton();

        for (int i = 0; i < enum_values.Length; i++) {
            option_button.GetPopup().AddItem(enum_values[i]);
        }

        int value = GetSetting<int>(where, type, instance_name);
        option_button.Select(value);

        void ApplyEnum() { ApplySetting(option_button.Selected, where, type, instance_name); }
        void ResetEnum() { option_button.Selected = default_value; }

        option_button.ItemSelected += (long new_value) => ValueChanged(container, (int)new_value, default_value, ResetEnum, ApplyEnum);

        container.AddChild(option_button);

        if (value != default_value) 
            AddResetButton(container, ResetEnum, ApplyEnum);
    }

    public void AddBoolSetting<T>(string full_name, T where, string instance_name, bool default_value) where T : ISettings {
        Type type = typeof(T);

        HBoxContainer container = CreateSettingsItem(full_name);

        CheckButton check_button = new CheckButton();
        check_button.ButtonPressed = GetSetting<bool>(where, type, instance_name);

        void ApplyBool() { ApplySetting(check_button.ButtonPressed, where, type, instance_name); }
        void ResetBool() { check_button.ButtonPressed = default_value; }

        check_button.Pressed += () => ValueChanged(container, check_button.ButtonPressed, default_value, ResetBool, ApplyBool);

        container.AddChild(check_button);

        if (check_button.ButtonPressed != default_value)
            AddResetButton(container, ResetBool, ApplyBool);
    }

    public void AddStringSetting<T>(string full_name, T where, string instance_name, string default_value) where T : ISettings {
        Type type = typeof(T);

        HBoxContainer container = CreateSettingsItem(full_name);

        LineEdit line_edit = new LineEdit();
        line_edit.Text = GetSetting<string>(where, type, instance_name);
        line_edit.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

        void ApplyString() { ApplySetting(line_edit.Text, where, type, instance_name); }
        void ResetString() { line_edit.Text = default_value; }

        line_edit.TextChanged += (string new_value) => ValueChanged(container, new_value, default_value, ResetString, ApplyString);

        container.AddChild(line_edit);

        if (line_edit.Text != "") 
            AddResetButton(container, ResetString, ApplyString);
    }

    public void AddFileLocationSetting<T>(string full_name, T where, string instance_name) where T : ISettings {
        Type type = typeof(T);

        HBoxContainer container = CreateSettingsItem(full_name);

        LineEdit line_edit = new LineEdit();
        line_edit.Text = GetSetting<string>(where, type, instance_name);
        line_edit.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;

        Button file_select_button = new Button();
        file_select_button.Icon = file_texture;
        file_select_button.ExpandIcon = true;
        file_select_button.TooltipText = "Open file manager to select location.";
        file_select_button.CustomMinimumSize = Vector2.One * 28;

        void ApplyFileLocation() { ApplySetting(line_edit.Text, where, type, instance_name); }
        void ResetFileLocation() { line_edit.Text = ""; }

        void FileLocation() {
            Globals.SetFileDialogFile();
            Globals.file_dialog.Popup();

            Globals.file_dialog.FileSelected += (string file) => {line_edit.Text = file; ValueChanged(container, file, "", ResetFileLocation, ApplyFileLocation);};
        }

        file_select_button.ButtonUp += FileLocation;

        line_edit.TextChanged += (string new_value) => ValueChanged(container, new_value, "", ResetFileLocation, ApplyFileLocation);

        container.AddChild(line_edit);
        container.AddChild(file_select_button);

        if (line_edit.Text != "") 
            AddResetButton(container, ResetFileLocation, ApplyFileLocation);
    }

    public void AddVector2Setting<T>(string full_name, T where, string instance_name, Vector2 default_value) where T : ISettings {
        Type type = typeof(T);

        HBoxContainer container = CreateSettingsItem(full_name);

        SpinBox spin_box_x = new SpinBox(), spin_box_y = new SpinBox();
        Vector2 value = GetSetting<Vector2>(where, type, instance_name);

        spin_box_x.Step = 0.01;
        spin_box_x.Value = value.X;

        spin_box_y.Step = 0.01;
        spin_box_y.Value = value.Y;

        void ApplyVector2() { ApplySetting(new Vector2((float)spin_box_x.Value, (float)spin_box_y.Value), where, type, instance_name); }
        void ResetVector2() { spin_box_x.Value = default_value.X; spin_box_y.Value = default_value.Y; }

        spin_box_x.ValueChanged += (double new_x_value) => ValueChanged(container, new Vector2((float)new_x_value, (float)spin_box_y.Value), default_value, ResetVector2, ApplyVector2);
        spin_box_y.ValueChanged += (double new_y_value) => ValueChanged(container, new Vector2((float)spin_box_x.Value, (float)new_y_value), default_value, ResetVector2, ApplyVector2);

        container.AddChild(spin_box_x);
        container.AddChild(spin_box_y);

        if (value != default_value) 
            AddResetButton(container, ResetVector2, ApplyVector2);
    }

            AddResetButton(container, ResetString, ApplyString);
    }

    public void Open() {
        Globals.file_dialog.Reparent(this);
        Show();
        SelectSettingsTab(0);
    }

    public void Close() {
        Globals.file_dialog.Reparent(Globals.self);
        Hide();
    }

    T GetSetting<T>(ISettings where, Type type, string name) {
        return (T)(type.GetField(name).GetValue(where));
    }

    public void ValueChanged<T>(HBoxContainer container, T new_value, T default_value, Action reset_setting, Action apply_setting) {
        ClearResetButton(container);

        if(!new_value.Equals(default_value))
            AddResetButton(container, reset_setting, apply_setting);

        apply_setting();
    }

    public void ApplySetting(Variant variant, ISettings where, Type type, string instance_name) {
        SetSetting(ref where, type, instance_name, variant);
        where.ApplySettings();
    }

    public void SetSetting<T>(ref T obj, Type type, string instance_name, Variant variant) where T : ISettings {
        if (Globals.save_data == null) return;

        FieldInfo field = type.GetField(instance_name, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
        
        if (field != null) {
            field.SetValue(obj, Convert.ChangeType(variant.Obj, field.FieldType));

            Globals.save_data.Save();
        }
        else GD.PrintErr($"Settings Field not found. Unable to set setting \'{instance_name}\'");
    }
}
