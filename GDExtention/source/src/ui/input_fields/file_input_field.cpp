#include "file_input_field.h"

#include <godot_cpp/core/memory.hpp>

using namespace godot;

void FileInputField::_bind_methods() {
    ClassDB::bind_method(D_METHOD("set_value", "p_value"), &FileInputField::set_value);
    ClassDB::bind_method(D_METHOD("get_value"), &FileInputField::get_value);
    
    ADD_PROPERTY(PropertyInfo(Variant::BOOL, "value"), "set_value", "get_value");

    ClassDB::bind_method(D_METHOD("set_filters", "p_default_value"), &FileInputField::set_filters);
    ClassDB::bind_method(D_METHOD("get_filters"), &FileInputField::get_filters);
    
    ADD_PROPERTY(PropertyInfo(Variant::BOOL, "filters"), "set_filters", "get_filter");

    ClassDB::bind_method(D_METHOD("get_line_edit"), &FileInputField::get_line_edit);

    ClassDB::bind_method(D_METHOD("open_file_dialog"), &FileInputField::open_file_dialog);

    ADD_SIGNAL(MethodInfo("value_changed", PropertyInfo(Variant::BOOL, "new_value")));
}

FileInputField::FileInputField() {
    HBoxContainer* container = memnew(HBoxContainer);

    _panel->add_child(container);

    container->set_anchors_preset(PRESET_FULL_RECT);

    _line_edit = memnew(LineEdit);

    _line_edit->set_text(_value);
    _line_edit->set_h_size_flags(SIZE_EXPAND_FILL);

    container->add_child(_line_edit);

    _line_edit->set_anchors_preset(PRESET_FULL_RECT);

    _line_edit->connect("text_changed", callable_mp(this, &FileInputField::_on_value_changed));

    _file_select_button = memnew(Button);

    //_file_select_button->set_button_icon(ApplicationManager::get_singleton()->get_theme_icon("folder_icon"));
    _file_select_button->set_expand_icon(true);
    _file_select_button->set_tooltip_text("Open file manager to select location.");
    _file_select_button->set_custom_minimum_size(Vector2(_MINIMUM_HEIGHT, _MINIMUM_HEIGHT));

    container->add_child(_file_select_button);

    _file_select_button->connect("button_up", callable_mp(this, &FileInputField::open_file_dialog));

    if (!is_default_value()) _add_reset_button();
}

FileInputField::~FileInputField() {}

void FileInputField::_reset_to_default_value() {
    _value = "";

    if(_line_edit) {
        _line_edit->set_text("");
    }
}

void FileInputField::_apply_value() {
    if(_line_edit) {
        _value = _line_edit->get_text();
        emit_signal("value_changed", this, _value);
    }
}

bool FileInputField::is_default_value() const{
    return _value == "";
}

String FileInputField::get_value() const{
    return _value;
}

void FileInputField::set_value(const String& p_value) {
    _value = p_value;
    emit_signal("value_changed", this, _value);

    if(_line_edit) _line_edit->set_text(_value);
}

PackedStringArray FileInputField::get_filters() const{
    return _filters;
}

void FileInputField::set_filters(const PackedStringArray& p_value) {
    _filters = p_value;
}

LineEdit* FileInputField::get_line_edit() const{
    return _line_edit;
}

void FileInputField::_set_file(const String& file) {
    set_value(file);
    _finish_file_select();
}

void FileInputField::_finish_file_select() {
    _file_dialog->queue_free();
    _file_dialog = nullptr;
}

void FileInputField::open_file_dialog() {
    if(_file_dialog != nullptr) return;

    _file_dialog = memnew(ThemeFileDialog);
    _file_dialog->set_filters(_filters);
    _file_dialog->set_title("Select " + get_full_name() + " file");
    _file_dialog->set_ok_button_text("Select " + get_full_name());
    
    add_child(_file_dialog);
    
    _file_dialog->connect("file_selected", callable_mp(this, &FileInputField::_set_file), CONNECT_ONE_SHOT);
    _file_dialog->connect("canceled", callable_mp(this, &FileInputField::_finish_file_select), CONNECT_ONE_SHOT);
    
    _file_dialog->popup();
}