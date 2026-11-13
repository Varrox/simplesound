#include "playlist.h"

#include <godot_cpp/classes/dir_access.hpp>

#include "tools.h"
#include "constants.h"

using namespace godot;

String Playlist::get_cover_path() {
    return _cover_path;
}

void Playlist::set_cover_path(const String& p_file_path) {
    _cover_path = p_file_path;
    _cover_texture = load_image_from_file(p_file_path);
}

PackedStringArray Playlist::get_tracks() {
    return _tracks;
}

void Playlist::set_tracks(PackedStringArray p_tracks) {
    _tracks_raw = p_tracks;
    _tracks = _process_tracks(p_tracks);
}

PackedStringArray Playlist::get_tracks_raw() {
    return _tracks_raw;
}

String Playlist::get_path() {
    if(_path.is_empty()) _path = USER_PLAYLISTS.path_join(get_proper_file_name(name) + ".json");
    return _path;
}

void Playlist::_get_tracks_from_directory(const String& p_directory_path, PackedStringArray& p_tracks) {
    PackedStringArray files = DirAccess::get_files_at(p_directory_path);
    PackedStringArray directories = DirAccess::get_directories_at(p_directory_path);

    for(const String& file : files) {
        if(is_valid_audio_file(file)) p_tracks.append(file);
    }

    for(const String& sub_directory : directories) {
        _get_tracks_from_directory(sub_directory, p_tracks);
    }
}