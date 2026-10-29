using Godot;

public partial class IntegerInputField : InputField {
    [Export] public int value, default_value, minimum_value = 0, maximum_value = int.MaxValue;

    [Signal]
    public delegate void ValueChangedEventHandler(int new_value);

    private SpinBox _spin_box;

    public SpinBox spin_box { get { return _spin_box; } }

    public override void _Ready() {
        base._Ready();

        _spin_box = new SpinBox {
            Value = value,
            MinValue = minimum_value,
            MaxValue = maximum_value
        };

        AddChild(_spin_box);

        _spin_box.ValueChanged += (double new_value) => _ValueChanged((int)new_value, default_value);

        if (value != default_value) 
            _AddResetButton();
    }

    protected override void _ApplyValue() {
        value = (int)_spin_box.Value;
        EmitSignal(SignalName.ValueChanged, _spin_box.Value);
    }

    protected override void _ResetToDefaultValue() {
        value = default_value;
        _spin_box.Value = default_value;
    }
}