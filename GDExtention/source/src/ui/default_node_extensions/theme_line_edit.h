#pragma once

#include <godot_cpp/classes/line_edit.hpp>

namespace godot {
    class ThemeLineEdit : public LineEdit {
    protected:
        static void _bind_methods();
    public:
        ThemeLineEdit();
        ~ThemeLineEdit();

        void _ready() override;
    };
};