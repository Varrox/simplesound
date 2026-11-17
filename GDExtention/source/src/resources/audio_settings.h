#pragma once 

#include "resources/settings.h"

namespace godot {
    class AudioSettings : public Settings {
    private:
    	const PackedStringArray PROPERTY_ROOM_SIZE = {"room_size"};
    	const PackedStringArray PROPERTY_DAMPING = {"damping"};
    	const PackedStringArray PROPERTY_SPREAD = {"spread"};
    	const PackedStringArray PROPERTY_HIGH_PASS = {"high_pass"};
    	const PackedStringArray PROPERTY_DRY = {"dry"};
    	const PackedStringArray PROPERTY_WET = {"wet"};

    	const PackedStringArray PROPERTY_HZ_32 = {"hz_32"};
    	const PackedStringArray PROPERTY_HZ_100 = {"hz_100"};
    	const PackedStringArray PROPERTY_HZ_320 = {"hz_320"};
    	const PackedStringArray PROPERTY_HZ_1000 = {"hz_1000"};
    	const PackedStringArray PROPERTY_HZ_3200 = {"hz_3200"};
    	const PackedStringArray PROPERTY_HZ_10000 = {"hz_10000"};
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