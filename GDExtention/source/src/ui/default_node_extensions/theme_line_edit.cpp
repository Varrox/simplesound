#include "theme_line_edit.h"

using namespace godot;

void ThemeLineEdit::_bind_methods() {}

ThemeLineEdit::ThemeLineEdit() {
    set_caret_blink_enabled(true);
}

ThemeLineEdit::~ThemeLineEdit() {}

void ThemeLineEdit::_ready() {
    connect("editing_toggled", callable_mp(this, &ThemeLineEdit::_emit_on_text_editing_toggled));
    //EditingToggled += ApplicationManager::get_singleton()->OnTextEditingToggled;
}

void ThemeLineEdit::_emit_on_text_editing_toggled() {
    
}