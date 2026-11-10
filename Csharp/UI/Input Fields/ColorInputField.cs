using Godot;

[GlobalClass]
public partial class ColorInputField : InputField {

    [Export] public Color value {
        set {
            _value = value;
            if(!IsInsideTree()) return;
            _theme_color_picker_button.Color = _value;
            EmitSignal(SignalName.ValueChanged, _value);
        }
        get { return _value; }
    }

    [Export] public Color default_value {
        set {
            _default_value = value;
            if(!IsInsideTree()) return;
            if (!IsDefaultValue()) _AddResetButton();
            else _ClearResetButton();
        }
        get { return _default_value; }
    }

    private Color _value, _default_value;

    [Signal]
    public delegate void ValueChangedEventHandler(Color new_value);

    private ThemeColorPickerButton _theme_color_picker_button;

    public ThemeColorPickerButton theme_color_picker_button { get { return _theme_color_picker_button; } }

    public override void _Ready() {
        base._Ready();

        _theme_color_picker_button = new ThemeColorPickerButton {
            Color = _value,
            CustomMinimumSize = new Vector2(MINIMUM_WIDTH * 3, MINIMUM_HEIGHT),
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };

        _panel.AddChild(_theme_color_picker_button);

        _theme_color_picker_button.SetAnchorsPreset(LayoutPreset.FullRect);

        _theme_color_picker_button.ColorChanged += (Color new_value) => _ValueChanged();

        if (!IsDefaultValue()) 
            _AddResetButton();
    }

    protected override void _ApplyValue() {
        _value = _theme_color_picker_button.Color;
        EmitSignal(SignalName.ValueChanged, _value);
    }

    protected override void _ResetToDefaultValue() {
        _value = _default_value;
        _theme_color_picker_button.Color = _default_value;
    }

    public override bool IsDefaultValue() {
        return _value == _default_value;
    }
}