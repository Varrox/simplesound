#include "integer_input_field.h"

#include <godot_cpp/core/memory.hpp>

using namespace godot;

void IntegerInputField::_bind_methods() {
    ClassDB::bind_method(D_METHOD("set_value", "p_value"), &IntegerInputField::set_value);
    ClassDB::bind_method(D_METHOD("get_value"), &IntegerInputField::get_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::INT, "value"), "set_value", "get_value");

    ClassDB::bind_method(D_METHOD("set_default_value", "p_default_value"), &IntegerInputField::set_default_value);
    ClassDB::bind_method(D_METHOD("get_default_value"), &IntegerInputField::get_default_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::INT, "default_value"), "set_default_value", "get_default_value");

    ClassDB::bind_method(D_METHOD("set_maximum_value", "p_maximum_value"), &IntegerInputField::set_maximum_value);
    ClassDB::bind_method(D_METHOD("get_maximum_value"), &IntegerInputField::get_maximum_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::INT, "maximum_value"), "set_maximum_value", "get_maximum_value");

    ClassDB::bind_method(D_METHOD("set_minimum_value", "p_minimum_value"), &IntegerInputField::set_minimum_value);
    ClassDB::bind_method(D_METHOD("get_minimum_value"), &IntegerInputField::get_minimum_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::INT, "minimum_value"), "set_minimum_value", "get_minimum_value");

    ClassDB::bind_method(D_METHOD("get_spin_box"), &IntegerInputField::get_spin_box);

    ADD_SIGNAL(MethodInfo("value_changed", PropertyInfo(Variant::INT, "new_value")));
}

IntegerInputField::IntegerInputField() {
    _spin_box = memnew(SpinBox);

    _spin_box->set_value(_value);
    _spin_box->set_min(_minimum_value);
    _spin_box->set_max(_maximum_value);
    _spin_box->set_h_size_flags(SIZE_EXPAND_FILL);

    _panel->add_child(_spin_box);

    _spin_box->set_anchors_preset(PRESET_FULL_RECT);

    _spin_box->connect("value_changed", callable_mp(this, &IntegerInputField::_on_value_changed));

    if (!is_default_value()) _add_reset_button();
}

IntegerInputField::~IntegerInputField() {}

void IntegerInputField::_reset_to_default_value() {
    _value = _default_value;

    if(_spin_box) {
        _spin_box->set_value(_default_value);
    }
}

void IntegerInputField::_apply_value() {
    if(_spin_box) {
        _value = (int)_spin_box->get_value();
        emit_signal("value_changed", this, _value);
    }
}

bool IntegerInputField::is_default_value() const{
    return _value == _default_value;
}

int IntegerInputField::get_value() const{
    return _value;
}

void IntegerInputField::set_value(int p_value) {
    _value = p_value;
    emit_signal("value_changed", this, _value);

    if(_spin_box) _spin_box->set_value(_value);
}

int IntegerInputField::get_default_value() const{
    return _default_value;
}

void IntegerInputField::set_default_value(int p_value) {
    _default_value = p_value;
    
    _clear_reset_button();
    if (!is_default_value()) _add_reset_button();
}

int IntegerInputField::get_minimum_value() const{
    return _minimum_value;
}

void IntegerInputField::set_minimum_value(int p_minimum_value) {
    _minimum_value = p_minimum_value;
    if(_spin_box) 
        _spin_box->set_min(_minimum_value);
}

int IntegerInputField::get_maximum_value() const{
    return _maximum_value;
}

void IntegerInputField::set_maximum_value(int p_maximum_value) {
    _maximum_value = p_maximum_value;
    if(_spin_box) 
        _spin_box->set_max(_maximum_value);
}

SpinBox* IntegerInputField::get_spin_box() const{
    return _spin_box;
}