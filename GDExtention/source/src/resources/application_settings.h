#pragma once 

#include "resources/settings.h"

#include "constants.h"

#include <godot_cpp/variant/string.hpp>

namespace godot {
    class ApplicationSettings : public Settings {
    private:
    	const PackedStringArray PROPERTY_YTDLP_LOCATION = {"ytdlp_location"};
		const PackedStringArray PROPERTY_DOWNLOAD_FORMAT = {"download_format"};
		const PackedStringArray PROPERTY_REMOVE_TRACK_WARNING = {"remove_track_warning"};
		const PackedStringArray PROPERTY_DISCORD_RICH_PRESENCE_ENABLED = {"discord_rich_presence_enabled"};
    public:
        ApplicationSettings();
        ~ApplicationSettings();

        void apply_settings() override;
        
        void write_json() override;
        void update_property(const String& p_property) override;

        // Downloader

        String ytdlp_location;

        DownloadFormat download_format;

        // Warnings

	    bool remove_track_warning;

	    // Discord

        bool discord_rich_presence_enabled;
    };
};