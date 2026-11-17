#pragma once

#include <godot_cpp/classes/color_picker_button.hpp>

namespace godot {
    class ThemeColorPickerButton : public ColorPickerButton {
    private:
        void _apply_customization();
    protected:
        static void _bind_methods();
    public:
        ThemeColorPickerButton();
        ~ThemeColorPickerButton();

        void _ready() override;
    };
};