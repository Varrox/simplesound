#pragma once

#include "ui/input_fields/input_field.h"

#include "ui/default_node_extensions/theme_file_dialog.h"

#include <godot_cpp/variant/packed_string_array.hpp>
#include <godot_cpp/classes/option_button.hpp>

namespace godot {
    class EnumInputField : public InputField {
        GDCLASS(EnumInputField, InputField);

    private:
        int _value;
        int _default_value;
        PackedStringArray _options;

        OptionButton* _option_button;

        inline void _on_value_changed(const int& p_new_value) {
            _value_changed();
        }
    protected:
        static void _bind_methods();

        void _reset_to_default_value() override;
        void _apply_value() override;
    public:
        EnumInputField();
        ~EnumInputField();

        bool is_default_value() const override;

        int get_value() const;
        void set_value(const int& p_value);

        int get_default_value() const;
        void set_default_value(int p_value);

        PackedStringArray get_options() const;
        void set_options(const PackedStringArray& p_value);

        OptionButton* get_option_button() const;
    };
};