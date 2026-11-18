#include "color_input_field.h"

#include <godot_cpp/core/memory.hpp>

using namespace godot;

void ColorInputField::_bind_methods() {
    ClassDB::bind_method(D_METHOD("set_value", "p_value"), &ColorInputField::set_value);
    ClassDB::bind_method(D_METHOD("get_value"), &ColorInputField::get_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::BOOL, "value"), "set_value", "get_value");

    ClassDB::bind_method(D_METHOD("set_default_value", "p_default_value"), &ColorInputField::set_default_value);
    ClassDB::bind_method(D_METHOD("get_default_value"), &ColorInputField::get_default_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::BOOL, "default_value"), "set_default_value", "get_default_value");

    ClassDB::bind_method(D_METHOD("get_theme_color_picker_button"), &ColorInputField::get_theme_color_picker_button);

    ADD_SIGNAL(MethodInfo("color_changed", PropertyInfo(Variant::BOOL, "new_value")));
}

ColorInputField::ColorInputField() {
    _theme_color_picker_button = memnew(ThemeColorPickerButton);

    _theme_color_picker_button->set_pick_color(_value);
    _theme_color_picker_button->set_h_size_flags(SIZE_EXPAND_FILL);

    _panel->add_child(_theme_color_picker_button);

    _theme_color_picker_button->set_anchors_preset(PRESET_FULL_RECT);

    _theme_color_picker_button->connect("color_changed", callable_mp(this, &ColorInputField::_on_value_changed));

    if (!is_default_value()) _add_reset_button();
}

ColorInputField::~ColorInputField() {}

void ColorInputField::_reset_to_default_value() {
    _value = _default_value;

    if(_theme_color_picker_button) {
        _theme_color_picker_button->set_pick_color(_default_value);
    }
}

void ColorInputField::_apply_value() {
    if(_theme_color_picker_button) {
        _value = _theme_color_picker_button->get_pick_color();
        emit_signal("color_changed", this, _value);
    }
}

bool ColorInputField::is_default_value() const{
    return _value == _default_value;
}

Color ColorInputField::get_value() const{
    return _value;
}

void ColorInputField::set_value(const Color& p_value) {
    _value = p_value;
    emit_signal("color_changed", this, _value);

    if(_theme_color_picker_button) _theme_color_picker_button->set_pick_color(_value);
}

Color ColorInputField::get_default_value() const{
    return _default_value;
}

void ColorInputField::set_default_value(const Color& p_value) {
    _default_value = p_value;
    
    _clear_reset_button();
    if (!is_default_value()) _add_reset_button();
}

ThemeColorPickerButton* ColorInputField::get_theme_color_picker_button() const{
    return _theme_color_picker_button;
}