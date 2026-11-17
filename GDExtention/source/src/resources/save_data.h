#pragma once

#include <godot_cpp/variant/packed_string_array.hpp>

#include "resources/settings.h"

namespace godot {
    class SaveData : public Settings {
    private:
        const PackedStringArray PROPERTY_PLAYIST_INDEX = {"playlist_index"};
        const PackedStringArray PROPERTY_TRACK_INDEX = {"track_index", "song_index"};
        const PackedStringArray PROPERTY_LOOKED_AT_PLAYLIST = {"looked_at_playlist"};

        const PackedStringArray PROPERTY_TIME = {"time"};
        const PackedStringArray PROPERTY_VOLUME = {"volume"};

        const PackedStringArray PROPERTY_SHUFFLED = {"shuffled"};

        const PackedStringArray PROPERTY_PLAYLISTS = {"playlists"};

        const PackedStringArray PROPERTY_APPLICATION_SETTINGS = {"application_settings"};
        const PackedStringArray PROPERTY_AUDIO_SETTINGS = {"audio_settings"};
        const PackedStringArray PROPERTY_GRAPHIC_SETTINGS = {"graphic_settings"};
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