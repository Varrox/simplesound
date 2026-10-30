using Godot;

[GlobalClass]
public partial class FileInputField : InputField {

    [Export] public string value {
        set {
            _value = value;
            if(!IsInsideTree()) return;
            _theme_line_edit.Text = _value;
            EmitSignal(SignalName.ValueChanged, _value);
        }
        get { return _value; }
    }

    private string _value;

    [Signal]
    public delegate void ValueChangedEventHandler(int new_value);

    private ThemeLineEdit _theme_line_edit;

    public ThemeLineEdit theme_line_edit { get { return _theme_line_edit; } }

    private Button _file_select_button;

    public Button file_select_button { get { return _file_select_button; } }

    public override void _Ready() {
        base._Ready();

        _theme_line_edit = new ThemeLineEdit {
            Text = _value,
            SizeFlagsHorizontal = SizeFlags.ExpandFill
        };

        AddChild(_theme_line_edit);

        _file_select_button = new Button {
            Icon = ApplicationManager.GetThemeIcon("folder_icon"),
            ExpandIcon = true,
            TooltipText = "Open file manager to select location.",
            CustomMinimumSize = Vector2.One * 28
        };

        _file_select_button.ButtonUp += OpenFileLocation;

        _theme_line_edit.TextChanged += (string new_value) => _ValueChanged(new_value, "");

        if (!_IsDefaultValue())
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

    protected override bool _IsDefaultValue() {
        return _value == "";
    }

    public void OpenFileLocation() {
        ThemeFileDialog file_dialog = new ThemeFileDialog();
        
        AddChild(file_dialog);

        file_dialog.SetFile();
        file_dialog.Popup();

        void SetFile(string file) {
            _theme_line_edit.Text = file;
            _ValueChanged(file, "");

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