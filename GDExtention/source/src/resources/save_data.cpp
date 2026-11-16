#include "save_data.h"

using namespace godot;

SaveData::SaveData() {
    playlist_index = 0;
    track_index = 0;
    looked_at_playlist = 0;

    time = 0.0f;
    volume = 0.0f;

    shuffled = false;
}

SaveData::~SaveData() {}

void SaveData::apply_settings() {

}
        
void SaveData::write_json() {

}

void SaveData::update_property(const String& p_property) {
    
}