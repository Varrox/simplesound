#include "application_settings.h"

using namespace godot;

ApplicationSettings::ApplicationSettings() {
	ytdlp_location = "";
	download_format = DownloadFormat::MP3;

	remove_track_warning = true;

	discord_rich_presence_enabled = true;
}

ApplicationSettings::~ApplicationSettings() {}