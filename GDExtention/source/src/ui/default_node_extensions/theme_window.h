#pragma once

#include <godot_cpp/classes/window.hpp>

namespace godot {
    class ThemeWindow : public Window {
    private:
        GDCLASS(ThemeWindow, Window);
    protected:
        static void _bind_methods();
    public:
        ThemeWindow();
        ~ThemeWindow();
    };
};