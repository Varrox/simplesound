using Godot;

[GlobalClass]
public partial class BooleanInputField : InputField {

    [Export] public bool value {
        set {
            _value = value;
            if(!IsInsideTree()) return;
            _check_box.ButtonPressed = _value;
            EmitSignal(SignalName.ValueChanged, _value);
        }
        get { return _value; }
    }

    [Export] public bool default_value {
        set {
            _default_value = value;
            if(!IsInsideTree()) return;
            if (!_IsDefaultValue()) _AddResetButton();
            else _ClearResetButton();
        }
        get { return _default_value; }
    }

    private bool _value, _default_value;

    [Signal]
    public delegate void ValueChangedEventHandler(bool new_value);

    private CheckBox _check_box;

    public CheckBox check_box { get { return _check_box; } }

    public override void _Ready() {
        base._Ready();

        _check_box = new CheckBox {
            ButtonPressed = _value
        };

        AddChild(_check_box);

        _check_box.Pressed += () => _ValueChanged(_check_box.ButtonPressed, _default_value);

        if (!_IsDefaultValue()) 
            _AddResetButton();
    }

    protected override void _ApplyValue() {
        _value = _check_box.ButtonPressed;
        EmitSignal(SignalName.ValueChanged, _value);
    }

    protected override void _ResetToDefaultValue() {
        _value = _default_value;
        _check_box.ButtonPressed = _default_value;
    }

    protected override bool _IsDefaultValue() {
        return _value == _default_value;
    }
}