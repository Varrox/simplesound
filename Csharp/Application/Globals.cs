using Godot;

[GlobalClass]
public partial class Globals : Node {
    [ExportGroup("Windows")]

    [Export] private AttributeEditor _attribute_editor{set{ attribute_editor = value; }get{ return attribute_editor; }}
    public static AttributeEditor attribute_editor;
}
