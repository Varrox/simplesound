using System;
using Godot;

public partial class EnumInputField : InputField {
    [Export] public int value, default_value;
    [Export] public string[] options;

    [Signal]
    public delegate void ValueChangedEventHandler(int new_value);

    private OptionButton _option_button;

    public OptionButton option_button { get { return _option_button; } }

    public override void _Ready() {
        base._Ready();

        _option_button = new OptionButton();

        for (int i = 0; i < options.Length; i++) {
            _option_button.GetPopup().AddItem(options[i]);
        }

        AddChild(_option_button);

        _option_button.ItemSelected += (long new_value) => _ValueChanged((int)new_value, default_value);

        if (value != default_value) 
            _AddResetButton();
    }

    protected override void _ApplyValue() {
        value = _option_button.Selected;
        EmitSignal(SignalName.ValueChanged, _option_button.Selected);
    }

    protected override void _ResetToDefaultValue() {
        value = default_value;
        _option_button.Selected = default_value;
    }

    public static EnumInputField CreateFromEnum<E>(E value, E default_value, bool capitalize_values = true) where E : struct, Enum {
        string[] enum_options = Enum.GetNames(typeof(E));

        for (int i = 0; i < enum_options.Length; i++) {
            string option_name = enum_options[i].ToLower();
            if (capitalize_values) option_name = option_name.Capitalize();
            enum_options[i] = option_name;
        }

        EnumInputField field = new EnumInputField {
            value = Convert.ToInt32(value),
            default_value = Convert.ToInt32(default_value),
            options = enum_options
        };

        return field;
    }
}