using Godot;
using System;
using Godot.Collections;

public partial class Settings : EditorWindow
{
    [Export] public Dictionary<Button, string[]> tabs;
    [Export] public VBoxContainer settings_container;

    Dictionary<string, Variant> values;

    public override void _Ready()
	{
		base._Ready();
        AddSetting("Blur Quality:q:sg:blur_quality:3");
    }

    public Control AddSetting(string setting)
    {
        int i1 = setting.Find(":") + 1, i2 = setting.Find(":", i1) + 1, i3 = setting.Find(":", i2) + 1, i4 = setting.Find(":", i3);
        string full_name = setting.Substring(0, i1);
        string type = setting.Substring(i1, i2 - i1 - 1);
        string where = setting.Substring(i2, i3 - i2 - 1);
        string name = setting.Substring(i3, i4 - i3 - 1);
        string default_value = setting.Substring(i4 + 1);

        HBoxContainer container = new HBoxContainer();
        settings_container.AddChild(container);

        Control input;

        switch (type)
        {
            case "int":
                input = new SpinBox();
                break;
            case "flt":
                input = new LineEdit();
                break;
            case "q":
                input = new SpinBox();
                (input as SpinBox).MaxValue = 3;
                (input as SpinBox).MinValue = 1;
                (input as SpinBox).Value = default_value.ToInt();
                break;
            case "str":
                input = new LineEdit();
                break;
            default:
                input = new LineEdit();
                break;
        }

        Label label = new Label();
        label.Text = full_name;

        container.AddChild(label);
        container.AddChild(input);

        if (where == "sg") // Shader Global
        {
            if(type == "q") 
            {
                GD.Print("Gup");
            }
        }
        else if (setting.StartsWith("c:")) // Constant
        {
            
        }

        return container;
    } 
}
