#pragma once

#include <godot_cpp/variant/packed_string_array.hpp>

#include "resources/settings.h"

namespace godot {
    class SaveData : public Settings {
    public:
        SaveData();
        ~SaveData();

        void apply_settings() override;
        
        void write_json() override;
        void update_property(const String& p_property) override;

        int playlist_index;
        int track_index;
        int looked_at_playlist;

        float time;
        float volume;

        bool shuffled;

        PackedStringArray playlists;
    };
};