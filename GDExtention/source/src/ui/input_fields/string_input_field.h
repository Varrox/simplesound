#pragma once

#include "ui/input_fields/input_field.h"

#include <godot_cpp/classes/line_edit.hpp>

namespace godot {
    class StringInputField : public InputField {
        GDCLASS(StringInputField, InputField);

    private:
        String _value;
        String _default_value;

        LineEdit* _line_edit;

        inline void _on_value_changed(const String& p_new_value) {
            _value_changed();
        }
    protected:
        static void _bind_methods();

        void _reset_to_default_value() override;
        void _apply_value() override;
    public:
        StringInputField();
        ~StringInputField();

        bool is_default_value() const override;

        String get_value() const;
        void set_value(const String& p_value);

        String get_default_value() const;
        void set_default_value(const String& p_value);

        LineEdit* get_line_edit() const;
    };
};