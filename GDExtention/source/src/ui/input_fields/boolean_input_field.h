#pragma once

#include "ui/input_fields/input_field.h"

#include <godot_cpp/classes/check_box.hpp>

namespace godot {
    class BooleanInputField : public InputField {
        GDCLASS(BooleanInputField, InputField);

    private:
        bool _value;
        bool _default_value;

        CheckBox* _check_box;

        inline void _on_value_changed(bool p_new_value) {
            _value_changed();
        }
    protected:
        static void _bind_methods();

        void _reset_to_default_value() override;
        void _apply_value() override;
    public:
        BooleanInputField();
        ~BooleanInputField();

        bool is_default_value() const override;

        bool get_value() const;
        void set_value(bool p_value);

        bool get_default_value() const;
        void set_default_value(bool p_value);

        CheckBox* get_check_box() const;
    };
};