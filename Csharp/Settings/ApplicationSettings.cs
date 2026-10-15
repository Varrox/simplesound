using Godot;
using System;
using System.Text.Json;

public class ApplicationSettings : ISettings
{
	// Downloader

    public string ytdlp_location;
	public DownloadFormat download_format;

	// Warnings

	public bool remove_track_warning;

	// SORT THESE BY AGE. DO NOT REMOVE OLD NAMES UNTIL ALPHA IS OVER, OR ALPHA v1.4.

	public static readonly string[] PROPERTY_YTDLP_LOCATION = ["ytdlp_location"];
	public static readonly string[] PROPERTY_DOWNLOAD_FORMAT = ["download_format"];

	public static readonly string[] PROPERTY_REMOVE_TRACK_WARNING = ["remove_track_warning"];

    public ApplicationSettings() {
		ytdlp_location = "";
		download_format = DownloadFormat.MP3;

		remove_track_warning = true;
	}

	public void ApplySettings() {
    }

	public void WriteJson(in Utf8JsonWriter writer) {
        writer.WriteString(PROPERTY_YTDLP_LOCATION[0], ytdlp_location);
		writer.WriteNumber(PROPERTY_DOWNLOAD_FORMAT[0], (int)download_format);

		writer.WriteBoolean(PROPERTY_REMOVE_TRACK_WARNING[0], remove_track_warning);
    }

	public void UpdateProperty(string property_name, ref Utf8JsonReader reader) {
		if (PROPERTY_YTDLP_LOCATION.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.String) ytdlp_location = reader.GetString() ?? "";
        }
		else if (PROPERTY_DOWNLOAD_FORMAT.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.Number) download_format = (DownloadFormat)reader.GetInt32();
        }
		else if (PROPERTY_REMOVE_TRACK_WARNING.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False) remove_track_warning = reader.GetBoolean();
        }
		
        reader.Skip();
    }
}