#pragma once

#include <godot_cpp/classes/h_box_container.hpp>
#include <godot_cpp/classes/label.hpp>
#include <godot_cpp/classes/button.hpp>
#include <godot_cpp/classes/panel.hpp>

namespace godot {
    class InputField : public HBoxContainer {
        GDCLASS(InputField, HBoxContainer);

    private:
        String _full_name;

        HBoxContainer* _field_name_container;
        Label* _full_name_label;

        Button* _reset_button;
    protected:
        static void _bind_methods();

        Panel* _panel;

        void _reset_button_pressed();
        void _add_reset_button();
        void _clear_reset_button();

        void _value_changed();

        virtual void _reset_to_default_value();
        virtual void _apply_value();

        const int _MINIMUM_WIDTH = 28;
        const int _MINIMUM_HEIGHT = 29;

    public:
        InputField();
        ~InputField();

        String get_full_name() const;
        void set_full_name(const String& p_full_name);

        virtual bool is_default_value() const;
    };
};