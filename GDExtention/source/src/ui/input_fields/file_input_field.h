#pragma once

#include "ui/input_fields/input_field.h"

#include "ui/default_node_extensions/theme_file_dialog.h"

#include <godot_cpp/variant/packed_string_array.hpp>
#include <godot_cpp/classes/line_edit.hpp>
#include <godot_cpp/classes/button.hpp>

namespace godot {
    class FileInputField : public InputField {
        GDCLASS(FileInputField, InputField);

    private:
        String _value;
        PackedStringArray _filters;

        LineEdit* _line_edit;
        Button* _file_select_button;

        ThemeFileDialog* _file_dialog;

        inline void _on_value_changed(const String& p_new_value) {
            _value_changed();
        }

        void _set_file(const String& file);
        void _finish_file_select();
    protected:
        static void _bind_methods();

        void _reset_to_default_value() override;
        void _apply_value() override;
    public:
        FileInputField();
        ~FileInputField();

        bool is_default_value() const override;

        String get_value() const;
        void set_value(const String& p_value);

        PackedStringArray get_filters() const;
        void set_filters(const PackedStringArray& p_value);

        LineEdit* get_line_edit() const;

        void open_file_dialog();
    };
};