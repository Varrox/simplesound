#pragma once

#include <godot_cpp/classes/popup_menu.hpp>

namespace godot {
    class ThemePopupMenu : public PopupMenu {
    public:
        ThemePopupMenu();
        ~ThemePopupMenu();

        void _ready() override;
    };
};