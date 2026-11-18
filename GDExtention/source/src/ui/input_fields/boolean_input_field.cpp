#include "boolean_input_field.h"

#include <godot_cpp/core/memory.hpp>

using namespace godot;

void BooleanInputField::_bind_methods() {
    ClassDB::bind_method(D_METHOD("set_value", "p_value"), &BooleanInputField::set_value);
    ClassDB::bind_method(D_METHOD("get_value"), &BooleanInputField::get_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::BOOL, "value"), "set_value", "get_value");

    ClassDB::bind_method(D_METHOD("set_default_value", "p_default_value"), &BooleanInputField::set_default_value);
    ClassDB::bind_method(D_METHOD("get_default_value"), &BooleanInputField::get_default_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::BOOL, "default_value"), "set_default_value", "get_default_value");

    ClassDB::bind_method(D_METHOD("get_check_box"), &BooleanInputField::get_check_box);

    ADD_SIGNAL(MethodInfo("pressed", PropertyInfo(Variant::BOOL, "new_value")));
}

BooleanInputField::BooleanInputField() {
    _check_box = memnew(CheckBox);

    _check_box->set_pressed(_value);
    _check_box->set_h_size_flags(SIZE_EXPAND_FILL);

    _panel->add_child(_check_box);

    _check_box->set_anchors_preset(PRESET_FULL_RECT);

    _check_box->connect("pressed", callable_mp(this, &BooleanInputField::_on_value_changed));

    if (!is_default_value()) _add_reset_button();
}

BooleanInputField::~BooleanInputField() {}

void BooleanInputField::_reset_to_default_value() {
    _value = _default_value;

    if(_check_box) {
        _check_box->set_pressed(_default_value);
    }
}

void BooleanInputField::_apply_value() {
    if(_check_box) {
        _value = _check_box->is_pressed();
        emit_signal("pressed", this, _value);
    }
}

bool BooleanInputField::is_default_value() const{
    return Math::abs(_value - _default_value) <= 0.005;
}

bool BooleanInputField::get_value() const{
    return _value;
}

void BooleanInputField::set_value(bool p_value) {
    _value = p_value;
    emit_signal("pressed", this, _value);

    if(_check_box) _check_box->set_pressed(_value);
}

bool BooleanInputField::get_default_value() const{
    return _default_value;
}

void BooleanInputField::set_default_value(bool p_value) {
    _default_value = p_value;
    
    _clear_reset_button();
    if (!is_default_value()) _add_reset_button();
}

CheckBox* BooleanInputField::get_check_box() const{
    return _check_box;
}