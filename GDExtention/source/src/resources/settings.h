#pragma once

#include <godot_cpp/variant/string.hpp>

namespace godot {
    class Settings {
    public:
        Settings();
        ~Settings();

        virtual void apply_settings();
        
        virtual void write_json();
        virtual void update_property(const String& p_property);
    };
}