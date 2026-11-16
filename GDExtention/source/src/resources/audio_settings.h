#pragma once 

#include "resources/settings.h"

namespace godot {
    class AudioSettings : public Settings {
    public:
        AudioSettings();
        ~AudioSettings();

        void apply_settings() override;
        
        void write_json() override;
        void update_property(const String& p_property) override;

        // Reverb

        float room_size;
        float damping;
        float spread;
        float high_pass;
        float dry;
        float wet;

        // EQ

        float hz_32;
        float hz_100;
        float hz_320;
        float hz_1000;
        float hz_3200;
        float hz_10000;
    };
};