using Godot;
using System;

public partial class ThemeColorPickerButton : ColorPickerButton
{
	public override void _Ready()
	{
		PickerCreated += CustomizeColorPicker;
	}

	void CustomizeColorPicker()
	{
		ColorPicker color_picker = GetPicker();

		if (color_picker != null) {
			MarginContainer margin_container = (MarginContainer)color_picker.GetChild(0, true);
			VBoxContainer container = (VBoxContainer)margin_container.GetChild(0, true);

			((Control)container.GetChild(container.GetChildCount() - 2, true)).Hide();   
		}

		PopupPanel popup_panel = GetPopup();

		if (popup_panel != null) {
			Panel panel = (Panel)popup_panel.GetChild(0, true);

			panel.AddThemeStyleboxOverride("panel", Globals.color_picker_panel_style);
		}
	}
}
