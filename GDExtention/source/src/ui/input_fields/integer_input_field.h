#pragma once

#include "ui/input_fields/input_field.h"

#include <godot_cpp/classes/spin_box.hpp>

namespace godot {
    class IntegerInputField : public InputField {
        GDCLASS(IntegerInputField, InputField);

    private:
        int _value;
        int _default_value;
        int _minimum_value;
        int _maximum_value;

        SpinBox* _spin_box;
    protected:
        static void _bind_methods();

        void _reset_to_default_value() override;
        void _apply_value() override;
    public:
        IntegerInputField();
        ~IntegerInputField();

        bool is_default_value() const override;

        int get_value() const;
        void set_value(int p_value);

        int get_default_value() const;
        void set_default_value(int p_value);

        int get_minimum_value() const;
        void set_minimum_value(int p_minimum_value);

        int get_maximum_value() const;
        void set_maximum_value(int p_maximum_value);

        SpinBox* get_spin_box() const;
    };
};