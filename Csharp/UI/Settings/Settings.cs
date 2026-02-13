using System;
using System.Reflection;
using Godot;
using Godot.Collections;

public partial class Settings : EditorWindow
{
    [Export] public Array<SettingsTab> settings_tabs;
    [Export] public VBoxContainer settings_container;
    [Export] public VBoxContainer tabs_container;
    [Export] public Texture2D reset_texture;

    [Export] public Button close_menu;
    Dictionary<string, Variant> values;

    static readonly string[] quality_levels = new[] { "Low", "Medium", "High" };

    public override void _Ready()
    {
        base._Ready();

        for (int i = 0; i < settings_tabs.Count; i++)
        {
            Button button = new Button();
            button.Text = settings_tabs[i].section_name;
            tabs_container.AddChild(button);
            button.Disabled = settings_tabs[i].disabled;
            int j = i;
            button.ButtonUp += () => AddSettings(j);
        }

        close_menu.ButtonUp += () => { Hide(); Globals.player.interrupted = false; };

        CallDeferred("AddSettings", 0);
    }

    public void AddSettings(int settings_tab)
    {
        for (int i = 0; i < settings_container.GetChildren().Count; i++)
        {
            settings_container.GetChild(i).QueueFree();
        }


        GD.Print(settings_tab);
        string[] strings = settings_tabs[settings_tab].settings;

        if (strings == null)
            return;

        for (int i = 0; i < strings.Length; i++)
        {
            AddSetting(strings[i]);
        }
    }

    protected (string, string, string, string, string) ParseSetting(string setting)
    {
        int i1 = setting.Find(":") + 1, i2 = setting.Find(":", i1) + 1, i3 = setting.Find(":", i2) + 1, i4 = setting.Find(":", i3);
        string full_name = setting.Substring(0, i1);
        string type = setting.Substring(i1, i2 - i1 - 1);
        string where = setting.Substring(i2, i3 - i2 - 1);
        string name = setting.Substring(i3, i4 - i3);
        string default_value = setting.Substring(i4 + 1);
        return (full_name, type, where, name, default_value);
    }

    public Control AddSetting(string setting)
    {
        (string full_name, string type, string where, string name, string default_value) = ParseSetting(setting);

        HBoxContainer container = new HBoxContainer();
        settings_container.AddChild(container);

        container.CustomMinimumSize = Vector2.Down * 28;

        Control[] input;

        switch (type)
        {
            case "int":
                input = new[] { new SpinBox() };
                (input[0] as SpinBox).Value = (int)(typeof(ApplicationMetadata).GetField(name).GetValue(Globals.save_data.application_metadata));
                break;
            case "flt":
                input = new[] { new SpinBox() };
                (input[0] as SpinBox).Step = 0.01;
                (input[0] as SpinBox).Value = (float)(typeof(ApplicationMetadata).GetField(name).GetValue(Globals.save_data.application_metadata));
                break;
            case "q":
                input = EnumSetting(quality_levels);
                break;
            case "bool":
                input = new[] { new CheckButton() };
                (input[0] as CheckButton).ButtonPressed = (bool)(typeof(ApplicationMetadata).GetField(name).GetValue(Globals.save_data.application_metadata));
                break;
            case "str":
                input = new[] { new LineEdit() };
                (input[0] as LineEdit).Text = (string)(typeof(ApplicationMetadata).GetField(name).GetValue(Globals.save_data.application_metadata));
                break;
            //case "fl":
            //    input = new[] { new Label(), new Button() };
            //    break;
            case "v2":
                input = new[] { new SpinBox(), new SpinBox() };
                Vector2 v = (Vector2)(typeof(ApplicationMetadata).GetField(name).GetValue(Globals.save_data.application_metadata));
                (input[0] as SpinBox).Prefix = "x:";
                (input[0] as SpinBox).MinValue = GetTree().Root.MinSize.X;
                (input[0] as SpinBox).MaxValue = 3000;
                (input[0] as SpinBox).Value = v.X;

                (input[1] as SpinBox).Prefix = "y:";
                (input[1] as SpinBox).MinValue = GetTree().Root.MinSize.Y;
                (input[1] as SpinBox).MaxValue = 3000;
                (input[1] as SpinBox).Value = v.Y;
                break;
            default:
                type = "str";
                input = new[] { new LineEdit() };
                (input[0] as LineEdit).Text = (string)(typeof(ApplicationMetadata).GetField(name).GetValue(Globals.save_data.application_metadata));
                break;
        }

        Label label = new Label();
        label.Text = full_name;

        container.AddChild(label);

        foreach (Control c in input)
        {
            container.AddChild(c);
        }

        if (type == "int" || type == "flt")
        {
            SpinBox i = (input[0] as SpinBox);
            i.ValueChanged += (double val) => NumberChanged(i, setting);
            if (i.Value != default_value.ToFloat())
            {
                AddReset(input, type, default_value, where, name);
            }
        }
        else if (type == "q")

        {
            OptionButton button = (input[0] as OptionButton);
            button.CallDeferred("select", (int)(typeof(ApplicationMetadata).GetField(name).GetValue(Globals.save_data.application_metadata)) - 1);
            button.ItemSelected += (long index) => EnumItemSelected(button, (int)index, setting);
            if (button.Selected != default_value.ToInt() - 1)
            {
                AddReset(input, type, default_value, where, name);
            }
        }
        else if (type == "bool")
        {
            CheckButton check_box = (input[0] as CheckButton);
            check_box.Pressed += () => BoolChanged(check_box, setting);
            if (check_box.ButtonPressed != (default_value == "true"))
            {
                AddReset(input, type, default_value, where, name);
            }
        }
        else if (type == "v2")
        {
            SpinBox x = (input[0] as SpinBox), y = (input[1] as SpinBox);
            x.ValueChanged += (double val) => Vector2Changed(x, y, setting);
            y.ValueChanged += (double val) => Vector2Changed(x, y, setting);
            Vector2 v = ParseVec2(default_value);
            if (x.Value != v.X || y.Value != v.Y)
            {
                AddReset(input, type, default_value, where, name);
            }
        }

        Globals.save_data.application_metadata.ApplySettings();

        return container;
    }

    Control[] EnumSetting(string[] _enum)
    {
        OptionButton b = new OptionButton();

        for (int i = 0; i < _enum.Length; i++)
        {
            b.GetPopup().AddItem(_enum[i]);
        }


        Control[] input = new[] { b };
        return input;
    }

    public Vector2 ParseVec2(string v2)
    {
        return new Vector2(v2.Substring(0, v2.Find(",")).ToInt(), v2.Substring(v2.Find(",") + 1).ToInt());
    }

    public void Vector2Changed(SpinBox x, SpinBox y, string setting)
    {
        (string full_name, string type, string where, string name, string default_value) = ParseSetting(setting);
        Vector2 v = ParseVec2(default_value);
        VariableSet(new[] { x, y }, x.Value != v.X || y.Value != v.Y, type, default_value, where, name);
    }

    public void BoolChanged(CheckButton btn, string setting)
    {
        (string full_name, string type, string where, string name, string default_value) = ParseSetting(setting);
        VariableSet(new[] { btn }, btn.ButtonPressed != (default_value == "true"), type, default_value, where, name);
    }

    public void NumberChanged(SpinBox spin, string setting)
    {
        (string full_name, string type, string where, string name, string default_value) = ParseSetting(setting);
        VariableSet(new[] { spin }, spin.Value != default_value.ToFloat(), type, default_value, where, name);
    }

    public void EnumItemSelected(OptionButton button, int index, string setting)
    {
        (string full_name, string type, string where, string name, string default_value) = ParseSetting(setting);

        VariableSet(new[] { button }, index != default_value.ToInt() - 1, type, default_value, where, name);
    }

    public void VariableSet(Control[] input, bool is_delta, string type, string default_value, string where, string name)
    {
        foreach (Node node in input[0].GetParent().GetChildren())
        {
            if (node is Button && System.Array.IndexOf(input, node) == -1) // Delete Reset button
            {
                node.QueueFree();
            }
        }

        if (is_delta) // Add reset button
        {
            AddReset(input, type, default_value, where, name);
        }


        ApplySetting(input, type, where, name);
    }

    public void AddReset(Control[] input, string type, string default_value, string where, string name)
    {
        Button button = new Button();
        button.Icon = reset_texture;
        button.ExpandIcon = true;
        input[input.Length - 1].AddSibling(button);
        button.CustomMinimumSize = Vector2.One * 28;
        button.ButtonUp += () => { SetInputToDefaultValue(input, type, default_value); button.QueueFree(); ApplySetting(input, type, where, name); };
    }

    public void ApplySetting(Control[] input, string type, string where, string name)
    {
        Variant variant;

        switch (type)
        {
            case "int":
                variant = (input[0] as SpinBox).Value;
                break;
            case "flt":
                variant = (input[0] as SpinBox).Value;
                break;
            case "q":
                variant = (input[0] as OptionButton).Selected + 1;
                break;
            case "bool":
                variant = (input[0] as CheckButton).ButtonPressed;
                break;
            case "str":
                variant = (input[0] as LineEdit).Text;
                break;
            case "v2":
                variant = new Vector2((float)(input[0] as SpinBox).Value, (float)(input[1] as SpinBox).Value);
                break;
            default:
                variant = (input[0] as LineEdit).Text;
                break;
        }


        switch (where)
        {
            case "sg": // Shader Global
                RenderingServer.GlobalShaderParameterSet(name, variant);
                SetConstant(name, variant, type);
                break;
            case "c": // Constant
                SetConstant(name, variant, type);
                Globals.save_data.application_metadata.ApplySettings();
                break;
        }
    }

    public void SetInputToDefaultValue(Control[] input, string type, string default_value)
    {
        switch (type)
        {
            case "int":
                (input[0] as SpinBox).Value = default_value.ToInt();
                break;
            case "flt":
                (input[0] as SpinBox).Value = default_value.ToFloat();
                break;
            case "q":
                (input[0] as OptionButton).Select(default_value.ToInt() - 1);
                break;
            case "bool":
                (input[0] as CheckButton).ButtonPressed = default_value == "true";
                break;
            case "str":
                (input[0] as LineEdit).Text = default_value;
                break;
            case "v2":
                Vector2 v = ParseVec2(default_value);
                (input[0] as SpinBox).Value = v.X;
                (input[1] as SpinBox).Value = v.Y;
                break;
            default:
                (input[0] as LineEdit).Text = default_value;
                break;
        }
    }

    public void SetConstant(string constant, Variant variant, string type)
    {
        if (Globals.self == null)
            return;

        FieldInfo field = typeof(ApplicationMetadata).GetField(constant, BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);


        object value = Convert.ChangeType(variant.Obj, field.FieldType);
        field.SetValue(Globals.save_data.application_metadata, value);

        GD.Print(Globals.save_data.application_metadata.blur_quality);

        Globals.save_data.Save();
    }
}
