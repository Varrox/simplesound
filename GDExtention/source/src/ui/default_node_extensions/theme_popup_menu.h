#pragma once

#include <godot_cpp/classes/popup_menu.hpp>

namespace godot {
    class ThemePopupMenu : public PopupMenu {
        GDCLASS(ThemePopupMenu, PopupMenu);
        
    protected:
        static void _bind_methods();
    public:
        ThemePopupMenu();
        ~ThemePopupMenu();

        void _ready() override;
    };
};