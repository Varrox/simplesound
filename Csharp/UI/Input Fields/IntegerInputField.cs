using Godot;

[GlobalClass]
public partial class IntegerInputField : InputField {

    [Export] public int value {
        set {
            _value = value;
            if(!IsInsideTree()) return;
            _spin_box.Value = _value;
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

    [Export] public int minimum_value {
        set {
            _minimum_value = value;
            if(!IsInsideTree()) return;
            _spin_box.MinValue = _minimum_value;
        }
        get { return _minimum_value; }
    }

    [Export] public int maximum_value {
        set {
            _maximum_value = value;
            if(!IsInsideTree()) return;
            _spin_box.MaxValue = _maximum_value;
        }
        get { return _maximum_value; }
    }

    private int _value, _default_value, _minimum_value = 0, _maximum_value = int.MaxValue;

    [Signal]
    public delegate void ValueChangedEventHandler(int new_value);

    private SpinBox _spin_box;

    public SpinBox spin_box { get { return _spin_box; } }

    public override void _Ready() {
        base._Ready();

        _spin_box = new SpinBox {
            Value = _value,
            MinValue = _minimum_value,
            MaxValue = _maximum_value
        };

        AddChild(_spin_box);

        _spin_box.ValueChanged += (double new_value) => _ValueChanged((int)new_value, _default_value);

        if (!_IsDefaultValue()) 
            _AddResetButton();
    }

    protected override void _ApplyValue() {
        _value = (int)_spin_box.Value;
        EmitSignal(SignalName.ValueChanged, _value);
    }

    protected override void _ResetToDefaultValue() {
        _value = _default_value;
        _spin_box.Value = _default_value;
    }

    protected override bool _IsDefaultValue() {
        return _value == _default_value;
    }
}