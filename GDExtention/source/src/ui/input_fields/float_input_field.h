#pragma once

#include "ui/input_fields/input_field.h"

#include <godot_cpp/classes/spin_box.hpp>

namespace godot {
    class FloatInputField : public InputField {
        GDCLASS(FloatInputField, InputField);

    private:
        float _value;
        float _default_value;
        float _minimum_value;
        float _maximum_value;

        SpinBox* _spin_box;

        inline void _on_value_changed(double p_new_value) {
            _value_changed();
        }
    protected:
        static void _bind_methods();

        void _reset_to_default_value() override;
        void _apply_value() override;
    public:
        FloatInputField();
        ~FloatInputField();

        bool is_default_value() const override;

        float get_value() const;
        void set_value(float p_value);

        float get_default_value() const;
        void set_default_value(float p_value);

        float get_minimum_value() const;
        void set_minimum_value(float p_minimum_value);

        float get_maximum_value() const;
        void set_maximum_value(float p_maximum_value);

        SpinBox* get_spin_box() const;
    };
};