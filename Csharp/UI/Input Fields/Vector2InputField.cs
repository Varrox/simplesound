using Godot;

[GlobalClass]
public partial class Vector2InputField : InputField {
    [Export] public Vector2 value {
        set {
            _value = value;
            if(!IsInsideTree()) return;
            _spin_box_x.Value = _value.X;
            _spin_box_y.Value = _value.Y;
            EmitSignal(SignalName.ValueChanged, _value);
        }
        get { return _value; }
    }

    [Export] public Vector2 default_value {
        set {
            _default_value = value;
            if(!IsInsideTree()) return;
            if (!_IsDefaultValue()) _AddResetButton();
            else _ClearResetButton();
        }
        get { return _default_value; }
    }

    [Export] public Vector2 minimum_value {
        set {
            _minimum_value = value;
            if(!IsInsideTree()) return;
            _spin_box_x.MinValue = _minimum_value.X;
            _spin_box_y.MinValue = _minimum_value.Y;
        }
        get { return _minimum_value; }
    }

    [Export] public Vector2 maximum_value {
        set {
            _maximum_value = value;
            if(!IsInsideTree()) return;
            _spin_box_x.MaxValue = _maximum_value.X;
            _spin_box_y.MaxValue = _maximum_value.Y;
        }
        get { return _maximum_value; }
    }

    private Vector2 _value, _default_value, _minimum_value = Vector2.Zero, _maximum_value = new Vector2(float.PositiveInfinity, float.PositiveInfinity);

    [Signal]
    public delegate void ValueChangedEventHandler(Vector2 new_value);

    private SpinBox _spin_box_x, _spin_box_y;

    public SpinBox spin_box_x { get { return _spin_box_x; } }
    public SpinBox spin_box_y { get { return _spin_box_y; } }

    public override void _Ready() {
        base._Ready();

        _spin_box_x = new SpinBox {
            Value = _value.X,
            Step = 0.01f,
            MinValue = _minimum_value.X,
            MaxValue = _maximum_value.X
        };

        _spin_box_y = new SpinBox {
            Value = _value.Y,
            Step = 0.01f,
            MinValue = _minimum_value.Y,
            MaxValue = _maximum_value.Y
        };

        AddChild(_spin_box_x);
        AddChild(_spin_box_y);

        _spin_box_x.ValueChanged += (double new_value) => _ValueChanged(new Vector2((float)new_value, _value.Y), _default_value);
        _spin_box_y.ValueChanged += (double new_value) => _ValueChanged(new Vector2(_value.X, (float)new_value), _default_value);

        if (!_IsDefaultValue()) 
            _AddResetButton();
    }

    protected override void _ApplyValue() {
        _value = new Vector2((float)_spin_box_x.Value, (float)_spin_box_y.Value);
        EmitSignal(SignalName.ValueChanged, _spin_box_x.Value);
    }

    protected override void _ResetToDefaultValue() {
        _value = _default_value;
        _spin_box_x.Value = _default_value.X;
        _spin_box_y.Value = _default_value.Y;
    }

    protected override bool _IsDefaultValue() {
        return _value == _default_value;
    }
}