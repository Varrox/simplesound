#pragma once

#include <godot_cpp/variant/string.hpp>

namespace godot {

struct TrackData {
    String title, artist, time;
    bool explicit_lyrics, corrupt;
    
    String share_link;

    TrackData();
    TrackData(String p_file);
    ~TrackData();
};

};