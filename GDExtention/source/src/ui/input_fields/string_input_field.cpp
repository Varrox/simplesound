#include "string_input_field.h"

#include <godot_cpp/core/memory.hpp>

using namespace godot;

void StringInputField::_bind_methods() {
    ClassDB::bind_method(D_METHOD("set_value", "p_value"), &StringInputField::set_value);
    ClassDB::bind_method(D_METHOD("get_value"), &StringInputField::get_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::BOOL, "value"), "set_value", "get_value");

    ClassDB::bind_method(D_METHOD("set_default_value", "p_default_value"), &StringInputField::set_default_value);
    ClassDB::bind_method(D_METHOD("get_default_value"), &StringInputField::get_default_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::BOOL, "default_value"), "set_default_value", "get_default_value");

    ClassDB::bind_method(D_METHOD("get_line_edit"), &StringInputField::get_line_edit);

    ADD_SIGNAL(MethodInfo("value_changed", PropertyInfo(Variant::BOOL, "new_value")));
}

StringInputField::StringInputField() {
    _line_edit = memnew(LineEdit);

    _line_edit->set_text(_value);
    _line_edit->set_h_size_flags(SIZE_EXPAND_FILL);

    _panel->add_child(_line_edit);

    _line_edit->set_anchors_preset(PRESET_FULL_RECT);

    _line_edit->connect("value_changed", callable_mp(this, &StringInputField::_on_value_changed));

    if (!is_default_value()) _add_reset_button();
}

StringInputField::~StringInputField() {}

void StringInputField::_reset_to_default_value() {
    _value = _default_value;

    if(_line_edit) {
        _line_edit->set_text(_default_value);
    }
}

void StringInputField::_apply_value() {
    if(_line_edit) {
        _value = _line_edit->get_text();
        emit_signal("value_changed", this, _value);
    }
}

bool StringInputField::is_default_value() const{
    return _value == _default_value;
}

String StringInputField::get_value() const{
    return _value;
}

void StringInputField::set_value(const String& p_value) {
    _value = p_value;
    emit_signal("value_changed", this, _value);

    if(_line_edit) _line_edit->set_text(_value);
}

String StringInputField::get_default_value() const{
    return _default_value;
}

void StringInputField::set_default_value(const String& p_value) {
    _default_value = p_value;
    
    _clear_reset_button();
    if (!is_default_value()) _add_reset_button();
}

LineEdit* StringInputField::get_line_edit() const{
    return _line_edit;
}