using Godot;
using Godot.Collections;
using System;

public partial class ContextMenu : Control
{
	[Export] public Array<ContextMenuOpener> sub_menus;
	public ContextMenuOpener opener;
}
