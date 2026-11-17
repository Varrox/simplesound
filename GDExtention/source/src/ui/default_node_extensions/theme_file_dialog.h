#pragma once

#include <godot_cpp/classes/file_dialog.hpp>

namespace godot {
    class ThemeFileDialog : public FileDialog {
    protected:
        static void _bind_methods();
    public:
        ThemeFileDialog();
        ~ThemeFileDialog();

        void set_tracks();
        void set_cover();
        void set_playlist();
        void set_zip();
    };
};