#include "float_input_field.h"

#include <godot_cpp/core/memory.hpp>
#include <godot_cpp/core/math.hpp>

using namespace godot;

void FloatInputField::_bind_methods() {
    ClassDB::bind_method(D_METHOD("set_value", "p_value"), &FloatInputField::set_value);
    ClassDB::bind_method(D_METHOD("get_value"), &FloatInputField::get_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "value"), "set_value", "get_value");

    ClassDB::bind_method(D_METHOD("set_default_value", "p_default_value"), &FloatInputField::set_default_value);
    ClassDB::bind_method(D_METHOD("get_default_value"), &FloatInputField::get_default_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "default_value"), "set_default_value", "get_default_value");

    ClassDB::bind_method(D_METHOD("set_maximum_value", "p_maximum_value"), &FloatInputField::set_maximum_value);
    ClassDB::bind_method(D_METHOD("get_maximum_value"), &FloatInputField::get_maximum_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "maximum_value"), "set_maximum_value", "get_maximum_value");

    ClassDB::bind_method(D_METHOD("set_minimum_value", "p_minimum_value"), &FloatInputField::set_minimum_value);
    ClassDB::bind_method(D_METHOD("get_minimum_value"), &FloatInputField::get_minimum_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::FLOAT, "minimum_value"), "set_minimum_value", "get_minimum_value");

    ClassDB::bind_method(D_METHOD("get_spin_box"), &FloatInputField::get_spin_box);

    ADD_SIGNAL(MethodInfo("value_changed", PropertyInfo(Variant::FLOAT, "new_value")));
}

FloatInputField::FloatInputField() {
    _spin_box = memnew(SpinBox);

    _spin_box->set_step(0.01f);
    _spin_box->set_value(_value);
    _spin_box->set_min(_minimum_value);
    _spin_box->set_max(_maximum_value);
    _spin_box->set_h_size_flags(SIZE_EXPAND_FILL);

    _panel->add_child(_spin_box);

    _spin_box->set_anchors_preset(PRESET_FULL_RECT);

    _spin_box->connect("value_changed", callable_mp(this, &FloatInputField::_on_value_changed));

    if (!is_default_value()) _add_reset_button();
}

FloatInputField::~FloatInputField() {}

void FloatInputField::_reset_to_default_value() {
    _value = _default_value;

    if(_spin_box) {
        _spin_box->set_value(_default_value);
    }
}

void FloatInputField::_apply_value() {
    if(_spin_box) {
        _value = (float)_spin_box->get_value();
        emit_signal("value_changed", this, _value);
    }
}

bool FloatInputField::is_default_value() const{
    return Math::abs(_value - _default_value) <= 0.005;
}

float FloatInputField::get_value() const{
    return _value;
}

void FloatInputField::set_value(float p_value) {
    _value = p_value;
    emit_signal("value_changed", this, _value);

    if(_spin_box) _spin_box->set_value(_value);
}

float FloatInputField::get_default_value() const{
    return _default_value;
}

void FloatInputField::set_default_value(float p_value) {
    _default_value = p_value;
    
    _clear_reset_button();
    if (!is_default_value()) _add_reset_button();
}

float FloatInputField::get_minimum_value() const{
    return _minimum_value;
}

void FloatInputField::set_minimum_value(float p_minimum_value) {
    _minimum_value = p_minimum_value;
    if(_spin_box) 
        _spin_box->set_min(_minimum_value);
}

float FloatInputField::get_maximum_value() const{
    return _maximum_value;
}

void FloatInputField::set_maximum_value(float p_maximum_value) {
    _maximum_value = p_maximum_value;
    if(_spin_box) 
        _spin_box->set_max(_maximum_value);
}

SpinBox* FloatInputField::get_spin_box() const{
    return _spin_box;
}