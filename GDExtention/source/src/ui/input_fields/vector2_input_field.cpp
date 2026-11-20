#include "vector2_input_field.h"

#include <godot_cpp/core/memory.hpp>
#include <godot_cpp/core/math.hpp>

using namespace godot;

void Vector2InputField::_bind_methods() {
    ClassDB::bind_method(D_METHOD("set_value", "p_value"), &Vector2InputField::set_value);
    ClassDB::bind_method(D_METHOD("get_value"), &Vector2InputField::get_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::VECTOR2, "value"), "set_value", "get_value");

    ClassDB::bind_method(D_METHOD("set_default_value", "p_default_value"), &Vector2InputField::set_default_value);
    ClassDB::bind_method(D_METHOD("get_default_value"), &Vector2InputField::get_default_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::VECTOR2, "default_value"), "set_default_value", "get_default_value");

    ClassDB::bind_method(D_METHOD("set_maximum_value", "p_maximum_value"), &Vector2InputField::set_maximum_value);
    ClassDB::bind_method(D_METHOD("get_maximum_value"), &Vector2InputField::get_maximum_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::VECTOR2, "maximum_value"), "set_maximum_value", "get_maximum_value");

    ClassDB::bind_method(D_METHOD("set_minimum_value", "p_minimum_value"), &Vector2InputField::set_minimum_value);
    ClassDB::bind_method(D_METHOD("get_minimum_value"), &Vector2InputField::get_minimum_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::VECTOR2, "minimum_value"), "set_minimum_value", "get_minimum_value");

    ClassDB::bind_method(D_METHOD("get_spin_box_x"), &Vector2InputField::get_spin_box_x);
    ClassDB::bind_method(D_METHOD("get_spin_box_y"), &Vector2InputField::get_spin_box_y);

    ADD_SIGNAL(MethodInfo("value_changed", PropertyInfo(Variant::VECTOR2, "new_value")));
}

Vector2InputField::Vector2InputField() {
    // X

    _spin_box_x = memnew(SpinBox);

    _spin_box_x->set_step(0.01f);
    _spin_box_x->set_value(_value.x);
    _spin_box_x->set_min(_minimum_value.x);
    _spin_box_x->set_max(_maximum_value.x);
    _spin_box_x->set_h_size_flags(SIZE_EXPAND_FILL);

    _panel->add_child(_spin_box_x);

    _spin_box_x->set_anchors_preset(PRESET_FULL_RECT);

    _spin_box_x->connect("value_changed", callable_mp(this, &Vector2InputField::_on_value_changed));

    // Y

    _spin_box_y = memnew(SpinBox);

    _spin_box_y->set_step(0.01f);
    _spin_box_y->set_value(_value.y);
    _spin_box_y->set_min(_minimum_value.y);
    _spin_box_y->set_max(_maximum_value.y);
    _spin_box_y->set_h_size_flags(SIZE_EXPAND_FILL);

    _panel->add_child(_spin_box_y);

    _spin_box_y->set_anchors_preset(PRESET_FULL_RECT);

    _spin_box_y->connect("value_changed", callable_mp(this, &Vector2InputField::_on_value_changed));

    if (!is_default_value()) _add_reset_button();
}

Vector2InputField::~Vector2InputField() {}

void Vector2InputField::_reset_to_default_value() {
    _value = _default_value;

    if(_spin_box_x) _spin_box_x->set_value(_default_value.x);
    if(_spin_box_y) _spin_box_y->set_value(_default_value.y);
}

void Vector2InputField::_apply_value() {
    if(_spin_box_x) _value.x = (float)_spin_box_x->get_value();
    if(_spin_box_y) _value.y = (float)_spin_box_y->get_value();
    
    emit_signal("value_changed", this, _value);
}

bool Vector2InputField::is_default_value() const{
    return Math::abs(_value.x - _default_value.x) <= 0.005 && Math::abs(_value.y - _default_value.y) <= 0.005;
}

Vector2 Vector2InputField::get_value() const{
    return _value;
}

void Vector2InputField::set_value(Vector2 p_value) {
    _value = p_value;
    emit_signal("value_changed", this, _value);

    if(_spin_box_x) _spin_box_x->set_value(_value.x);
    if(_spin_box_y) _spin_box_y->set_value(_value.y);
}

Vector2 Vector2InputField::get_default_value() const{
    return _default_value;
}

void Vector2InputField::set_default_value(Vector2 p_value) {
    _default_value = p_value;
    
    _clear_reset_button();
    if (!is_default_value()) _add_reset_button();
}

Vector2 Vector2InputField::get_minimum_value() const{
    return _minimum_value;
}

void Vector2InputField::set_minimum_value(Vector2 p_minimum_value) {
    _minimum_value = p_minimum_value;

    if(_spin_box_x) _spin_box_x->set_min(_minimum_value.x);
    if(_spin_box_y) _spin_box_y->set_min(_minimum_value.y);
}

Vector2 Vector2InputField::get_maximum_value() const{
    return _maximum_value;
}

void Vector2InputField::set_maximum_value(Vector2 p_maximum_value) {
    _maximum_value = p_maximum_value;

    if(_spin_box_x) _spin_box_x->set_max(_maximum_value.x);
    if(_spin_box_y) _spin_box_y->set_max(_maximum_value.y);
}

SpinBox* Vector2InputField::get_spin_box_x() const{
    return _spin_box_x;
}

SpinBox* Vector2InputField::get_spin_box_y() const{
    return _spin_box_y;
}