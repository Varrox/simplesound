using Godot;

[GlobalClass]
public partial class Globals : Node {
    [ExportGroup("Windows")]

    [Export] private AttributeEditor _attribute_editor{set{ attribute_editor = value; }get{ return attribute_editor; }}
    public static AttributeEditor attribute_editor;

    [ExportGroup("Packed Scenes")]

    [Export] private PackedScene _confirmation_window{set{ confirmation_window = value; }get{ return confirmation_window; }}
    public static PackedScene confirmation_window;
}
