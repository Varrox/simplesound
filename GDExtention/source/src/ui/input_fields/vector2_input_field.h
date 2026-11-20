#pragma once

#include "ui/input_fields/input_field.h"

#include <godot_cpp/variant/vector2.hpp>
#include <godot_cpp/classes/spin_box.hpp>

namespace godot {
    class Vector2InputField : public InputField {
        GDCLASS(Vector2InputField, InputField);

    private:
        Vector2 _value;
        Vector2 _default_value;
        Vector2 _minimum_value;
        Vector2 _maximum_value;

        SpinBox* _spin_box_x;
        SpinBox* _spin_box_y;
        
        inline void _on_value_changed(double p_new_value) {
            _value_changed();
        }
    protected:
        static void _bind_methods();

        void _reset_to_default_value() override;
        void _apply_value() override;
    public:
        Vector2InputField();
        ~Vector2InputField();

        bool is_default_value() const override;

        Vector2 get_value() const;
        void set_value(Vector2 p_value);

        Vector2 get_default_value() const;
        void set_default_value(Vector2 p_value);

        Vector2 get_minimum_value() const;
        void set_minimum_value(Vector2 p_minimum_value);

        Vector2 get_maximum_value() const;
        void set_maximum_value(Vector2 p_maximum_value);

        SpinBox* get_spin_box_x() const;
        SpinBox* get_spin_box_y() const;
    };
};