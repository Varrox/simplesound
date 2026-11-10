using System.Linq;
using Godot;
using Godot.Collections;

[GlobalClass]
public partial class FileInputField : InputField {

    [Export] public string value {
        set {
            _value = value ?? "";
            if(!IsInsideTree()) return;
            _theme_line_edit.Text = _value;
            EmitSignal(SignalName.ValueChanged, _value);
        }
        get { return _value; }
    }

    [Export] public Array<string> filters;

    private string _value = "";

    [Signal]
    public delegate void ValueChangedEventHandler(string new_value);

    private ThemeLineEdit _theme_line_edit;

    public ThemeLineEdit theme_line_edit { get { return _theme_line_edit; } }

    private Button _file_select_button;

    public Button file_select_button { get { return _file_select_button; } }

    public override void _Ready() {
        base._Ready();

        HBoxContainer container = new HBoxContainer {};

        _panel.AddChild(container);

        container.SetAnchorsPreset(LayoutPreset.FullRect);

        _theme_line_edit = new ThemeLineEdit {
            Text = _value,
            PlaceholderText = "...",
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };

        container.AddChild(_theme_line_edit);

        _theme_line_edit.SetAnchorsPreset(LayoutPreset.FullRect);

        _file_select_button = new Button {
            Icon = ApplicationManager.GetThemeIcon("folder_icon"),
            ExpandIcon = true,
            TooltipText = "Open file manager to select location.",
            CustomMinimumSize = new Vector2(MINIMUM_HEIGHT, MINIMUM_HEIGHT)
        };

        container.AddChild(_file_select_button);

        _file_select_button.ButtonUp += OpenFileLocation;

        _theme_line_edit.TextChanged += (string new_value) => _ValueChanged();

        if (!IsDefaultValue())
            _AddResetButton();
    }

    protected override void _ApplyValue() {
        _value = _theme_line_edit.Text;
        EmitSignal(SignalName.ValueChanged, _value);
    }

    protected override void _ResetToDefaultValue() {
        _value = "";
        _theme_line_edit.Text = "";
    }

    public override bool IsDefaultValue() {
        return (_value ?? "").Equals("");
    }

    public void OpenFileLocation() {
        ThemeFileDialog file_dialog = new ThemeFileDialog {
            Filters = filters == null ? null : filters.ToArray(),
            Title = $"Select {full_name} file",
            OkButtonText = $"Select {full_name}",
        };
        
        AddChild(file_dialog);
        
        file_dialog.Popup();

        void SetFile(string file) {
            _theme_line_edit.Text = file;
            _ValueChanged();

            Finish();
        }

        void Finish() {
            file_dialog.FileSelected -= SetFile;
            file_dialog.Canceled -= Finish;
            
            file_dialog.QueueFree();
        }

        file_dialog.FileSelected += SetFile;
        file_dialog.Canceled += Finish;
    }
}