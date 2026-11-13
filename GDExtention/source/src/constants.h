#pragma once 

#include <godot_cpp/variant/string.hpp>
#include <godot_cpp/variant/packed_string_array.hpp>
#include <godot_cpp/templates/vector.hpp>

using namespace godot;

const Vector<String> EXECUTABLE_FORMATS {
#ifdef _WIN32 || _WIN64
    "exe"
#else
#endif
};

const Vector<String> DOWNLOAD_FORMATS {
    "mp3",
    "wav"
};

enum DownloadFormat : uint32_t {
    MP3 = 0,
    WAV = 1
};

const Vector<String> PLAYABLE_FORMATS {
    "mp3",
    "wav",
    "ogg"
};

const Vector<String> IMAGE_FORMATS {
    "png", 
    "jpg", 
    "jpeg", 
    "bmp", 
    "dds", 
    "svg", 
    "tga", 
    "webp"
};

const Vector<String> QUALITY_LEVELS {
    "Low",
    "Medium",
    "High"
};

enum QualityLevel : uint32_t {
    LOW = 0,
    MEDIUM = 1,
    HIGH = 2
};

const String SOURCE = "https://github.com/Varrox/simplesound";
const String LATEST_RELEASE = "https://api.github.com/repos/Varrox/simplesound/releases/latest";

const char* DOT = "\u00b7";

enum AudioEffects : uint32_t {
    SPECTRUM_ANALIZER = 0,
    REVERB = 1,
    EQ = 2
};

const String USER_DATA = String(getenv("APPDATA")).path_join("simplesound");

// Folders

const String USER_PLAYLISTS = USER_DATA.path_join("playlists");
const String USER_TRACKS = USER_DATA.path_join("tracks");
const String USER_PLAYLIST_COVERS = USER_DATA.path_join("data").path_join("playlist_covers");

// Files

const String USER_SAVEDATA = USER_DATA.path_join("savedata.json");