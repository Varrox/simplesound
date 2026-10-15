using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using System.Text;

public class SaveData : ISettings {
	public int playlist_index, song_index, looked_at_playlist;
	public float time, volume;

	public bool shuffled;

	public List<string> playlists;
	
	public ApplicationSettings application_settings;
	public CloudSettings cloud_settings;
	public AudioSettings audio_settings;
	public GraphicSettings graphic_settings;

    // SORT THESE BY AGE. DO NOT REMOVE OLD NAMES UNTIL ALPHA IS OVER, OR ALPHA v1.4.

    public static readonly string[] PROPERTY_PLAYIST_INDEX = ["playlist_index"];
    public static readonly string[] PROPERTY_TRACK_INDEX = ["track_index", "song_index"];
    public static readonly string[] PROPERTY_LOOKED_AT_PLAYLIST = ["looked_at_playlist"];

    public static readonly string[] PROPERTY_TIME = ["time"];
    public static readonly string[] PROPERTY_VOLUME = ["volume"];

    public static readonly string[] PROPERTY_SHUFFLED = ["shuffled"];

    public static readonly string[] PROPERTY_PLAYLISTS = ["playlists"];

    public static readonly string[] PROPERTY_APPLICATION_SETTINGS = ["application_settings"];
    public static readonly string[] PROPERTY_CLOUD_SETTINGS = ["cloud_settings"];
    public static readonly string[] PROPERTY_AUDIO_SETTINGS = ["audio_settings"];
    public static readonly string[] PROPERTY_GRAPHIC_SETTINGS = ["graphic_settings"];

    public SaveData() {
        playlist_index = 0;
        song_index = 0;
        looked_at_playlist = 0;

        time = 0.0f;
        volume = 0.0f;

        shuffled = false;

        application_settings = new ApplicationSettings();
        cloud_settings = new CloudSettings();
        audio_settings = new AudioSettings();
        graphic_settings = new GraphicSettings();
    }

    public void ApplySettings() {
        application_settings.ApplySettings();
        cloud_settings.ApplySettings();
        audio_settings.ApplySettings();
        graphic_settings.ApplySettings();
    }

    public void WriteJson(in Utf8JsonWriter writer) {
        // Playback info

        writer.WriteNumber(PROPERTY_PLAYIST_INDEX[0], playlist_index);
        writer.WriteNumber(PROPERTY_TRACK_INDEX[0], song_index);
        writer.WriteNumber(PROPERTY_LOOKED_AT_PLAYLIST[0], looked_at_playlist);

        writer.WriteNumber(PROPERTY_TIME[0], time);
        writer.WriteNumber(PROPERTY_VOLUME[0], volume);

        writer.WriteBoolean(PROPERTY_SHUFFLED[0], shuffled);

        // Playlists
        
        writer.WriteStartArray(PROPERTY_PLAYLISTS[0]);

        if (playlists != null) {
            foreach(string playlist_path in playlists) {
                writer.WriteStringValue(playlist_path);
            }
        }

        writer.WriteEndArray();

        // Settings

        writer.WriteStartObject(PROPERTY_APPLICATION_SETTINGS[0]);
        application_settings.WriteJson(writer);
        writer.WriteEndObject();

        writer.WriteStartObject(PROPERTY_CLOUD_SETTINGS[0]);
        cloud_settings.WriteJson(writer);
        writer.WriteEndObject();

        writer.WriteStartObject(PROPERTY_AUDIO_SETTINGS[0]);
        audio_settings.WriteJson(writer);
        writer.WriteEndObject();

        writer.WriteStartObject(PROPERTY_GRAPHIC_SETTINGS[0]);
        graphic_settings.WriteJson(writer);
        writer.WriteEndObject();
    }

    public byte[] CreateJson() {
        JsonWriterOptions writer_options = JsonHelper.CreateJsonWriterOptions();

        using MemoryStream stream = new MemoryStream();
        using Utf8JsonWriter writer = new Utf8JsonWriter(stream, writer_options);

        writer.WriteStartObject();

        WriteJson(writer);

        writer.WriteEndObject();
        writer.Flush();

        return stream.ToArray();
    }

    public void Save() {
        File.WriteAllBytes(Constants.USER_SAVEDATA, CreateJson());
    }

    public void UpdateProperty(string property_name, ref Utf8JsonReader reader) {
        if (PROPERTY_PLAYIST_INDEX.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.Number) playlist_index = reader.GetInt32();
            else {return;}
        }
        else if (PROPERTY_TRACK_INDEX.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.Number) song_index = reader.GetInt32();
            else {return;}
        }
        else if (PROPERTY_LOOKED_AT_PLAYLIST.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.Number) looked_at_playlist = reader.GetInt32();
            else {return;}
        }
        else if (PROPERTY_TIME.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.Number) time = (float)reader.GetDouble();
            else {return;}
        }
        else if (PROPERTY_VOLUME.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.Number) volume = (float)reader.GetDouble();
            else {return;}
        }
        else if (PROPERTY_SHUFFLED.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False) shuffled = reader.GetBoolean();
            else {return;}
        }
        else if (PROPERTY_PLAYLISTS.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.StartArray) playlists = JsonHelper.GetStringList(ref reader);
        }
        else if (PROPERTY_APPLICATION_SETTINGS.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.StartObject) application_settings = JsonHelper.LoadSettingsFromJson<ApplicationSettings>(ref reader);
        }
        else if (PROPERTY_CLOUD_SETTINGS.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.StartObject) cloud_settings = JsonHelper.LoadSettingsFromJson<CloudSettings>(ref reader);
        }
        else if (PROPERTY_AUDIO_SETTINGS.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.StartObject) audio_settings = JsonHelper.LoadSettingsFromJson<AudioSettings>(ref reader);
        }
        else if (PROPERTY_GRAPHIC_SETTINGS.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.StartObject) graphic_settings = JsonHelper.LoadSettingsFromJson<GraphicSettings>(ref reader);
        }
        else {
            reader.Skip();
        }
    }

    public static SaveData LoadFromJson(in ReadOnlySpan<byte> json_bytes) {
        Utf8JsonReader reader = new Utf8JsonReader(json_bytes, JsonHelper.CreateJsonReaderOptions());

        reader.Read();
        
        SaveData save_data = JsonHelper.LoadSettingsFromJson<SaveData>(ref reader);

        return save_data;
    }

    public static SaveData GetSaveData() {
        foreach (string folder_path in new[] { Constants.USER_PLAYLISTS, Constants.USER_TRACKS, Constants.USER_PLAYLIST_COVERS }) { // Create folders
            if (!Directory.Exists(folder_path)) Directory.CreateDirectory(folder_path);
        }

        SaveData save_data;

        if (!File.Exists(Constants.USER_SAVEDATA)) {
            save_data = new();
            save_data.Save();
        }
        else {
            ReadOnlySpan<byte> bytes = JsonHelper.LoadJsonBytes(Constants.USER_SAVEDATA);
            save_data = LoadFromJson(bytes);
        }

        return save_data;
    }
}