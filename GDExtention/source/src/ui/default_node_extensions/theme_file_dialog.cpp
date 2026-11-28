#include "theme_file_dialog.h"

#include <godot_cpp/core/class_db.hpp>

#include "tools.h"
#include "constants.h"

using namespace godot;

void ThemeFileDialog::_bind_methods() {
    ClassDB::bind_method(D_METHOD("set_tracks"), &ThemeFileDialog::set_tracks);
    ClassDB::bind_method(D_METHOD("set_cover"), &ThemeFileDialog::set_cover);
    ClassDB::bind_method(D_METHOD("set_playlist"), &ThemeFileDialog::set_playlist);
    ClassDB::bind_method(D_METHOD("set_zip"), &ThemeFileDialog::set_zip);
}

ThemeFileDialog::ThemeFileDialog() {
    set_use_native_dialog(true);
	set_show_hidden_files(true);

    set_access(ACCESS_FILESYSTEM);
	set_initial_position(Window::WINDOW_INITIAL_POSITION_CENTER_PRIMARY_SCREEN);
}

ThemeFileDialog::~ThemeFileDialog() {}

void ThemeFileDialog::set_tracks() {
    set_filters(Tools::get_filters(PLAYABLE_FORMATS));
    set_file_mode(FileMode::FILE_MODE_OPEN_FILES);
    set_ok_button_text("Import audio files");
	set_title("Select Audio files");
}

void ThemeFileDialog::set_cover() {
    set_filters(Tools::get_filters(IMAGE_FORMATS));
    set_file_mode(FileMode::FILE_MODE_OPEN_FILE);
    set_ok_button_text("Import Cover");
    set_title("Select Cover");
}

void ThemeFileDialog::set_playlist() {
    set_filters({ "*.json"});
    set_file_mode(FileMode::FILE_MODE_OPEN_FILE);
    set_ok_button_text("Import Playlist");
    set_title("Select Playlist");
}

void ThemeFileDialog::set_zip() {
    set_filters({ "*.zip"});
    set_file_mode(FileMode::FILE_MODE_OPEN_FILE);
    set_ok_button_text("Import Zip Archive");
    set_title("Select Zip Archive");
}