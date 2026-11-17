#include "theme_line_edit.h"

using namespace godot;

void ThemeLineEdit::_bind_methods() {}

ThemeLineEdit::ThemeLineEdit() {
    set_caret_blink_enabled(true);
}

ThemeLineEdit::~ThemeLineEdit() {}

void ThemeLineEdit::_ready() {
    //EditingToggled += ApplicationManager::get_singleton()->OnTextEditingToggled;
}