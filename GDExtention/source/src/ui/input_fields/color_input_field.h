#pragma once

#include "ui/input_fields/input_field.h"

#include "ui/default_node_extensions/theme_color_picker_button.h"

namespace godot {
    class ColorInputField : public InputField {
        GDCLASS(ColorInputField, InputField);

    private:
        Color _value;
        Color _default_value;

        ThemeColorPickerButton* _theme_color_picker_button;

        inline void _on_value_changed(const Color& p_new_value) {
            _value_changed();
        }
    protected:
        static void _bind_methods();

        void _reset_to_default_value() override;
        void _apply_value() override;
    public:
        ColorInputField();
        ~ColorInputField();

        bool is_default_value() const override;

        Color get_value() const;
        void set_value(const Color& p_value);

        Color get_default_value() const;
        void set_default_value(const Color& p_value);

        ThemeColorPickerButton* get_theme_color_picker_button() const;
    };
};