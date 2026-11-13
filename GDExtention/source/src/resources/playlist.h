#pragma once

#include <godot_cpp/variant/string.hpp>
#include <godot_cpp/variant/packed_string_array.hpp>
#include <godot_cpp/classes/texture2d.hpp>

namespace godot {
    enum PlaylistType : uint32_t {
        DEFAULT = 0,
        ALBUM = 1
    };

    struct Playlist {
        String name;
        String artist;

        String get_cover_path();
        void set_cover_path(const String& p_file_path);

        PackedStringArray get_tracks();
        void set_tracks(PackedStringArray p_tracks);

        PackedStringArray get_tracks_raw();

        String get_path();

        PlaylistType playlist_type;
    private:
        String _cover_path;

        PackedStringArray _tracks_raw;
        PackedStringArray _tracks;

        Ref<Texture2D> _cover_texture;

        String _path;

        static void _get_tracks_from_directory(const String& p_directory_path, PackedStringArray& p_tracks);
        static PackedStringArray _process_tracks(PackedStringArray tracks);

        const PackedStringArray PROPERTY_NAME = {"name"};
        const PackedStringArray PROPERTY_ARTIST = {"artist"};

        const PackedStringArray PROPERTY_COVER_ART = {"cover_art", "cover"};

        const PackedStringArray PROPERTY_PLAYLIST_TYPE = {"playlist_type", "type"};

        const PackedStringArray PROPERTY_TRACKS = {"tracks", "songs"};
    };
};