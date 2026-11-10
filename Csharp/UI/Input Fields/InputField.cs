using System;
using Godot;

[GlobalClass]
public partial class InputField : HBoxContainer {
    [Export] public string full_name
    {
        set {
            field = value;
            if (_full_name_label != null) _full_name_label.Text = $"{value}:";
        }
        get;
    }

    private HBoxContainer _field_name_container;
    private Label _full_name_label;
    protected Panel _panel;

    private Button _reset_button;

    public const int MINIMUM_WIDTH = 28, MINIMUM_HEIGHT = 29;

    public override void _Ready() {
        AddThemeConstantOverride("separation", 5);

        _field_name_container = new HBoxContainer {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill
        };

        _field_name_container.AddThemeConstantOverride("separation", 5);

        AddChild(_field_name_container);

        _full_name_label = new Label {
            MouseFilter = MouseFilterEnum.Ignore,
            Text = $"{full_name}:",
            VerticalAlignment = VerticalAlignment.Center,
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill,
            TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis
        };

        _field_name_container.AddChild(_full_name_label);

        _panel = new Panel {
            SizeFlagsHorizontal = SizeFlags.ExpandFill,
            SizeFlagsVertical = SizeFlags.ExpandFill
        };

        AddChild(_panel);

        CustomMinimumSize = new Vector2(MINIMUM_WIDTH, MINIMUM_HEIGHT);
    }

    protected virtual void _ResetToDefaultValue() {
        
    }

    protected virtual void _ApplyValue() {
        
    }

    protected virtual bool _IsDefaultValue() {
        return true;
    }

    protected void _AddResetButton() {
        _ClearResetButton();

        _reset_button = new Button {
            Icon = ApplicationManager.GetThemeIcon("reload_icon"),
            ExpandIcon = true,
            CustomMinimumSize = Vector2.One * 28
        };

        _field_name_container.AddChild(_reset_button);

        _reset_button.SizeFlagsHorizontal = SizeFlags.ShrinkEnd;

        void ResetButtonPressed() {
            _ResetToDefaultValue();
            _ApplyValue();
            if (_IsDefaultValue())
                _ClearResetButton();
        }

        _reset_button.ButtonUp += ResetButtonPressed;
    }

    protected void _ClearResetButton() {
        if(_reset_button != null) {
            _reset_button.QueueFree();
            _reset_button = null;
        }
    }

    protected void _ValueChanged() {
        _ApplyValue();

        _ClearResetButton();

        if(!_IsDefaultValue())
            _AddResetButton();
    }
}