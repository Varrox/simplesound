using System;
using Godot;

[GlobalClass]
public partial class EnumInputField : InputField {

    [Export] public int value {
        set {
            _value = value;
            if(!IsInsideTree()) return;
            _option_button.Select(_value);
            EmitSignal(SignalName.ValueChanged, _value);
        }
        get { return _value; }
    }

    [Export] public int default_value {
        set {
            _default_value = value;
            if(!IsInsideTree()) return;
            if (!_IsDefaultValue()) _AddResetButton();
            else _ClearResetButton();
        }
        get { return _default_value; }
    }

    [Export] public string[] options {
        set {
            _options = value;
            _option_button.GetPopup().Clear();

            for (int i = 0; i < _options.Length; i++) {
                _option_button.GetPopup().AddItem(_options[i]);
            }
        }
        get { return _options; }
    }

    private int _value, _default_value;
    private string[] _options;

    [Signal]
    public delegate void ValueChangedEventHandler(int new_value);

    private OptionButton _option_button;

    public OptionButton option_button { get { return _option_button; } }

    public override void _Ready() {
        base._Ready();

        _option_button = new OptionButton();

        for (int i = 0; i < _options.Length; i++) {
            _option_button.GetPopup().AddItem(_options[i]);
        }

        _option_button.Select(value);

        AddChild(_option_button);

        _option_button.ItemSelected += (long new_value) => _ValueChanged((int)new_value, _default_value);

        if (!_IsDefaultValue()) 
            _AddResetButton();
    }

    protected override void _ApplyValue() {
        _value = _option_button.Selected;
        EmitSignal(SignalName.ValueChanged, _value);
    }

    protected override void _ResetToDefaultValue() {
        _value = _default_value;
        _option_button.Select(_default_value);
    }

    protected override bool _IsDefaultValue() {
        return _value == _default_value;
    }

    public static EnumInputField CreateFromEnum<E>(E value, E default_value, bool capitalize_values = true) where E : struct, Enum {
        string[] enum_options = Enum.GetNames(typeof(E));

        for (int i = 0; i < enum_options.Length; i++) {
            string option_name = enum_options[i].ToLower();
            if (capitalize_values) option_name = option_name.Capitalize();
            enum_options[i] = option_name;
        }

        EnumInputField field = new EnumInputField {
            _value = Convert.ToInt32(value),
            _default_value = Convert.ToInt32(default_value),
            _options = enum_options
        };

        return field;
    }
}