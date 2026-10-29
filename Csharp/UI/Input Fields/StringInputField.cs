using Godot;

public partial class StringInputField : InputField {
    [Export] public string value, default_value;

    [Signal]
    public delegate void ValueChangedEventHandler(int new_value);

    private ThemeLineEdit _theme_line_edit;

    public ThemeLineEdit theme_line_edit { get { return _theme_line_edit; } }

    public override void _Ready() {
        base._Ready();

        _theme_line_edit = new ThemeLineEdit {
            Text = value
        };

        AddChild(_theme_line_edit);

        _theme_line_edit.TextChanged += (string new_value) => _ValueChanged(new_value, default_value);

        if (value != default_value) 
            _AddResetButton();
    }

    protected override void _ApplyValue() {
        value = _theme_line_edit.Text;
        EmitSignal(SignalName.ValueChanged, _theme_line_edit.Text);
    }

    protected override void _ResetToDefaultValue() {
        value = default_value;
        _theme_line_edit.Text = default_value;
    }
}