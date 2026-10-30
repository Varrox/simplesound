using Godot;

[GlobalClass]
public partial class StringInputField : InputField {

    [Export] public string value {
        set {
            _value = value;
            if(!IsInsideTree()) return;
            _theme_line_edit.Text = _value;
            EmitSignal(SignalName.ValueChanged, _value);
        }
        get { return _value; }
    }

    [Export] public string default_value {
        set {
            _default_value = value;
            if(!IsInsideTree()) return;
            if (!_IsDefaultValue()) _AddResetButton();
            else _ClearResetButton();
        }
        get { return _default_value; }
    }

    private string _value, _default_value;

    [Signal]
    public delegate void ValueChangedEventHandler(int new_value);

    private ThemeLineEdit _theme_line_edit;

    public ThemeLineEdit theme_line_edit { get { return _theme_line_edit; } }

    public override void _Ready() {
        base._Ready();

        _theme_line_edit = new ThemeLineEdit {
            Text = _value
        };

        AddChild(_theme_line_edit);

        _theme_line_edit.TextChanged += (string new_value) => _ValueChanged(new_value, _default_value);

        if (!_IsDefaultValue()) 
            _AddResetButton();
    }

    protected override void _ApplyValue() {
        _value = _theme_line_edit.Text;
        EmitSignal(SignalName.ValueChanged, _value);
    }

    protected override void _ResetToDefaultValue() {
        _value = _default_value;
        _theme_line_edit.Text = _default_value;
    }

    protected override bool _IsDefaultValue() {
        return _value == _default_value;
    }
}