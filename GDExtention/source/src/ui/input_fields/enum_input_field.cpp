#include "enum_input_field.h"

#include <godot_cpp/core/memory.hpp>
#include <godot_cpp/classes/popup_menu.hpp>

using namespace godot;

void EnumInputField::_bind_methods() {
    ClassDB::bind_method(D_METHOD("set_value", "p_value"), &EnumInputField::set_value);
    ClassDB::bind_method(D_METHOD("get_value"), &EnumInputField::get_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::BOOL, "value"), "set_value", "get_value");

    ClassDB::bind_method(D_METHOD("set_options", "p_default_value"), &EnumInputField::set_options);
    ClassDB::bind_method(D_METHOD("get_options"), &EnumInputField::get_options);
    
    ADD_PROPERTY(PropertyInfo(Variant::BOOL, "filters"), "set_options", "get_filter");

    ClassDB::bind_method(D_METHOD("get_option_button"), &EnumInputField::get_option_button);

    ADD_SIGNAL(MethodInfo("value_changed", PropertyInfo(Variant::BOOL, "new_value")));
}

EnumInputField::EnumInputField() {
    HBoxContainer* container = memnew(HBoxContainer);

    _panel->add_child(container);

    container->set_anchors_preset(PRESET_FULL_RECT);

    _option_button = memnew(OptionButton);

    _option_button->select(_value);
    _option_button->set_h_size_flags(SIZE_EXPAND_FILL);

    for (const String& option : _options) {
        _option_button->get_popup()->add_item(option);
    }

    container->add_child(_option_button);

    _option_button->set_anchors_preset(PRESET_FULL_RECT);

    _option_button->connect("item_selected", callable_mp(this, &EnumInputField::_on_value_changed));

    if (!is_default_value()) _add_reset_button();
}

EnumInputField::~EnumInputField() {}

void EnumInputField::_reset_to_default_value() {
    _value = _default_value;

    if(_option_button) {
        _option_button->set_text("");
    }
}

void EnumInputField::_apply_value() {
    if(_option_button) {
        _value = _option_button->get_selected();
        emit_signal("text_changed", this, _value);
    }
}

bool EnumInputField::is_default_value() const{
    return _value == _default_value;
}

int EnumInputField::get_value() const{
    return _value;
}

void EnumInputField::set_value(const int& p_value) {
    _value = p_value;
    emit_signal("text_changed", this, _value);

    if(_option_button) _option_button->select(_value);
}

int EnumInputField::get_default_value() const{
    return _default_value;
}

void EnumInputField::set_default_value(int p_value) {
    _default_value = p_value;
    
    _clear_reset_button();
    if (!is_default_value()) _add_reset_button();
}

PackedStringArray EnumInputField::get_options() const{
    return _options;
}

void EnumInputField::set_options(const PackedStringArray& p_value) {
    _options = p_value;

    _option_button->get_popup()->clear();

    for (const String& option : _options) {
        _option_button->get_popup()->add_item(option);
    }

    if (_value >= _options.size()) {
        _reset_to_default_value();
    }
}

OptionButton* EnumInputField::get_option_button() const{
    return _option_button;
}