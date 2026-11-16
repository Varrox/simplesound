#pragma once 

#include <godot_cpp/variant/vector2i.hpp>

#include "resources/settings.h"
#include "constants.h"

namespace godot {
    class GraphicsSettings : public Settings {
    public:
        GraphicsSettings();
        ~GraphicsSettings();

        void apply_settings() override;
        
        void write_json() override;
        void update_property(const String& p_property) override;

        QualityLevel blur_quality;

        bool vsync;
        bool reduce_fps_on_lose_focus;
        int max_fps;

        Vector2i main_display_size;
    };
};