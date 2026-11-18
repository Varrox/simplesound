#include "input_field.h"

#include <godot_cpp/core/memory.hpp>

using namespace godot;

void InputField::_bind_methods() {
    ClassDB::bind_method(D_METHOD("is_default_value"), &InputField::is_default_value);
}

InputField::InputField() {
    add_theme_constant_override("separation", 5);

    _field_name_container = memnew(HBoxContainer);

    _field_name_container->set_h_size_flags(SIZE_EXPAND_FILL);
    _field_name_container->set_v_size_flags(SIZE_EXPAND_FILL);

    _field_name_container->add_theme_constant_override("separation", 5);

    add_child(_field_name_container);

    _full_name_label = memnew(Label);

    _full_name_label->set_mouse_filter(MouseFilter::MOUSE_FILTER_IGNORE);
    _full_name_label->set_text(_full_name + ":");
    _full_name_label->set_vertical_alignment(VerticalAlignment::VERTICAL_ALIGNMENT_CENTER);
    _full_name_label->set_h_size_flags(SIZE_EXPAND_FILL);
    _full_name_label->set_v_size_flags(SIZE_EXPAND_FILL);
    _full_name_label->set_text_overrun_behavior(TextServer::OverrunBehavior::OVERRUN_TRIM_ELLIPSIS);

    _field_name_container->add_child(_full_name_label);

    _panel = memnew(Panel);

    _panel->set_h_size_flags(SIZE_EXPAND_FILL);
    _panel->set_v_size_flags(SIZE_EXPAND_FILL);

    add_child(_panel);

    set_custom_minimum_size(Vector2(_MINIMUM_WIDTH, _MINIMUM_HEIGHT));
}

InputField::~InputField() {}

void InputField::_reset_button_pressed() {
    _reset_to_default_value();
    _apply_value();
    if (is_default_value())
        _clear_reset_button();
}

void InputField::_add_reset_button() {
    _clear_reset_button();

    _reset_button = memnew(Button);
    //_reset_button->set_button_icon(ApplicationManager::get_singleton()->get_theme_icon("reload_icon"));
    _reset_button->set_expand_icon(true);
    _reset_button->set_custom_minimum_size(Vector2(_MINIMUM_HEIGHT, _MINIMUM_HEIGHT));

    _field_name_container->add_child(_reset_button);

    _reset_button->set_h_size_flags(SIZE_SHRINK_END);

    _reset_button->connect("button_up", callable_mp(this, &InputField::_reset_button_pressed));
}

void InputField::_clear_reset_button() {
    if(_reset_button != nullptr) {
        _reset_button->queue_free();
        _reset_button = nullptr;
    }
}

void InputField::_value_changed() {
    _apply_value();

    _clear_reset_button();

    if(!is_default_value())
        _add_reset_button();
}

void InputField::_reset_to_default_value() {}

void InputField::_apply_value() {}

bool InputField::is_default_value() { return true; }