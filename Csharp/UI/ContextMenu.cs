using Godot;
using Godot.Collections;

public partial class ContextMenu : Control {
	[Export] public Array<ContextMenuOpener> sub_menus;
	public ContextMenuOpener opener;
}
