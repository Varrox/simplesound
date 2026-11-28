#include "theme_color_picker_button.h"

#include <godot_cpp/classes/color_picker.hpp>
#include <godot_cpp/classes/margin_container.hpp>
#include <godot_cpp/classes/v_box_container.hpp>
#include <godot_cpp/classes/popup_panel.hpp>
#include <godot_cpp/classes/panel.hpp>

#include "application/application_manager.h"

using namespace godot;

void ThemeColorPickerButton::_bind_methods() {}

ThemeColorPickerButton::ThemeColorPickerButton() {}
ThemeColorPickerButton::~ThemeColorPickerButton() {}

void ThemeColorPickerButton::_ready() {
    
}

void ThemeColorPickerButton::_apply_customization() {
    ColorPicker* color_picker = get_picker();

	if (color_picker != nullptr) {
		MarginContainer* margin_container = Object::cast_to<MarginContainer>(color_picker->get_child(0, true));
		VBoxContainer* container = Object::cast_to<VBoxContainer>(margin_container->get_child(0, true));

		Object::cast_to<Control>(container->get_child(container->get_child_count() - 2, true))->hide();   
	}

	PopupPanel* popup_panel = get_popup();

	if (popup_panel != nullptr) {
		Panel* panel = Object::cast_to<Panel>(popup_panel->get_child(0, true));
        
		panel->add_theme_stylebox_override("panel", ApplicationManager::get_singleton()->get_theme_stylebox("color_picker_panel_stylebox"));
	}
}