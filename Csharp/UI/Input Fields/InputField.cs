using System;
using Godot;

[GlobalClass]
public partial class InputField : HBoxContainer {
    [Export] public string full_name
    {
        set {
            field = value;
            if (_full_name_label != null) _full_name_label.Text = value + ":";
        }
        get;
    }
    private Label _full_name_label;
    private Button _reset_button;

    public const int MINIMUM_WIDTH = 28, MINIMUM_HEIGHT = 29;

    public override void _Ready() {
        _full_name_label = new Label {
            MouseFilter = MouseFilterEnum.Ignore,
            Text = full_name + ":"
        };

        CustomMinimumSize = new Vector2(MINIMUM_WIDTH, MINIMUM_HEIGHT);

        AddChild(_full_name_label);
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

        AddChild(_reset_button);

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

    protected void _ValueChanged<T>(T new_value, T default_value) {
        _ClearResetButton();

        if(!new_value.Equals(default_value))
            _AddResetButton();

        _ApplyValue();
    }
}