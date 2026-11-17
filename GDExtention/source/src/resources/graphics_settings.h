#pragma once 

#include <godot_cpp/variant/vector2i.hpp>

#include "resources/settings.h"
#include "constants.h"

namespace godot {
    class GraphicsSettings : public Settings {
    private:
        const PackedStringArray PROPERTY_BLUR_QUALITY = {"blur_quality"};
	    const PackedStringArray PROPERTY_VSYNC = {"vsync"};
        const PackedStringArray PROPERTY_MAX_FPS = {"max_fps"};
        const PackedStringArray PROPERTY_REDUCE_FPS_ON_LOSE_FOCUS = {"reduce_fps_on_lose_focus"};
        const PackedStringArray PROPERTY_MAIN_DISPLAY_SIZE = {"main_display_size"};
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