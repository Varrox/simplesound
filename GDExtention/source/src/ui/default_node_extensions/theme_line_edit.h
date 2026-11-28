#pragma once

#include <godot_cpp/classes/line_edit.hpp>

namespace godot {
    class ThemeLineEdit : public LineEdit {
        GDCLASS(ThemeLineEdit, LineEdit);
        
    private:
        void _emit_on_text_editing_toggled();
    protected:
        static void _bind_methods();
    public:
        ThemeLineEdit();
        ~ThemeLineEdit();

        void _ready() override;
    };
};