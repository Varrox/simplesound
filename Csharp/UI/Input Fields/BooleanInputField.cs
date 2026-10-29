using Godot;

public partial class BooleanInputField : InputField {
    [Export] public bool value, default_value;

    [Signal]
    public delegate void ValueChangedEventHandler(bool new_value);

    private CheckBox _check_box;

    public CheckBox check_box { get { return _check_box; } }

    public override void _Ready() {
        base._Ready();

        _check_box = new CheckBox {
            ButtonPressed = value
        };

        AddChild(_check_box);

        _check_box.Pressed += () => _ValueChanged(_check_box.ButtonPressed, default_value);

        if (value != default_value) 
            _AddResetButton();
    }

    protected override void _ApplyValue() {
        value = _check_box.ButtonPressed;
        EmitSignal(SignalName.ValueChanged, _check_box.ButtonPressed);
    }

    protected override void _ResetToDefaultValue() {
        value = default_value;
        _check_box.ButtonPressed = default_value;
    }
}