using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using Godot;

public class Playlist
{
    public string name;
    public string artist;

    public string cover {
        set {
            field = value;
            _cover_texture = ConvertToGodot.LoadImageFromFile(value);
        }
        get;
    }

    public PlaylistType type;

    public List<string> tracks { get { return _tracks; } set {
            _tracks_raw = RemoveInvaidTracks(value);
            _tracks = IncludeDirectoryTracks(_tracks_raw);
        } 
    }

    public CustomInfo custom_info;

    public string path {
        get {
            if(_path == null) { _path = Path.Combine(Constants.USER_PLAYLISTS, $"{name.Replace('\\', '-').Replace('/', '-').Replace(':', '-')}.json"); }
            return _path;
        }
    }

    public Texture2D cover_texture { get { return _cover_texture; } }

    private List<string> _tracks;
    private List<string> _tracks_raw; // For saving folders correctly without expanding them.

    private Texture2D _cover_texture;

    private string _path;

    // SORT THESE BY AGE. DO NOT REMOVE OLD NAMES UNTIL ALPHA IS OVER, OR ALPHA v1.4.

    public static readonly string[] PROPERTY_NAME = ["name"];
    public static readonly string[] PROPERTY_ARTIST = ["artist"];

    public static readonly string[] PROPERTY_COVER_ART = ["cover_art", "cover"];

    public static readonly string[] PROPERTY_PLAYLIST_TYPE = ["playlist_type", "type"];

    public static readonly string[] PROPERTY_TRACKS = ["tracks", "songs"];

    public enum PlaylistType {
        Default,
        Album
    }

    public Playlist() {
        name = "";
        artist = "";
        cover = "";

        type = PlaylistType.Default;

        custom_info = new CustomInfo();
    }

    public byte[] CreateJson() {
        var writer_options = JsonHelper.CreateJsonWriterOptions();

        using var stream = new MemoryStream();
        using var writer = new Utf8JsonWriter(stream, writer_options);

        writer.WriteStartObject();

        writer.WriteString(PROPERTY_NAME[0], name);
        writer.WriteString(PROPERTY_ARTIST[0], artist);

        writer.WriteString(PROPERTY_COVER_ART[0], cover);

        writer.WriteNumber(PROPERTY_PLAYLIST_TYPE[0], (int)type);
        
        writer.WriteStartArray(PROPERTY_TRACKS[0]);

        if (_tracks_raw != null) {
            foreach(string track_path in _tracks_raw) {
                writer.WriteStringValue(track_path);
            }
        }

        writer.WriteEndArray();

        writer.WriteEndObject();
        writer.Flush();

        return stream.ToArray();
    }

    /// Saves to path
    public string Save() {
        File.WriteAllBytes(path, CreateJson());

        return path;
    }

    public static Playlist LoadFromJson(in ReadOnlySpan<byte> json_bytes) {
        Playlist playlist = new Playlist();

        Utf8JsonReader reader = new Utf8JsonReader(json_bytes, JsonHelper.CreateJsonReaderOptions());

        reader.Read();

        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        while(reader.Read()) {
            if (reader.TokenType == JsonTokenType.EndObject) break;

            if (reader.TokenType == JsonTokenType.PropertyName) {
                string property_name = reader.GetString();

                reader.Read();

                if (PROPERTY_NAME.Contains(property_name)) {
                    if (reader.TokenType == JsonTokenType.String) playlist.name = reader.GetString() ?? "";
                }
                else if (PROPERTY_ARTIST.Contains(property_name)) {
                    if (reader.TokenType == JsonTokenType.String) playlist.artist = reader.GetString() ?? "";
                }
                else if (PROPERTY_COVER_ART.Contains(property_name)) {
                    if (reader.TokenType == JsonTokenType.String) playlist.cover = reader.GetString() ?? "";
                }
                else if (PROPERTY_PLAYLIST_TYPE.Contains(property_name)) {
                    if (reader.TokenType == JsonTokenType.Number) playlist.type = (PlaylistType)reader.GetInt32();
                }
                else if (PROPERTY_TRACKS.Contains(property_name)) {
                    if (reader.TokenType == JsonTokenType.StartArray) playlist.tracks = JsonHelper.GetStringList(ref reader);
                }
                else {
                    reader.Skip();
                }
            }
        }

        return playlist;
    }

    public static Playlist LoadFromFile(string path) {
        string json_text = File.ReadAllText(path);
        ReadOnlySpan<byte> bytes = Encoding.UTF8.GetBytes(json_text);

        Playlist playlist = LoadFromJson(bytes);

        return playlist;
    }

    public void DeleteFile() {
        File.Delete(path);

        Globals.save_data.playlists.Remove(path);

        Globals.save_data.Save();
    }

    public static void GetTracksFromDirectory(ref List<string> tracks, string directory) {
        foreach(string file in Directory.GetFiles(directory)) {
            if(Tools.ValidAudioFile(file)) tracks.Add(file);
        }

        foreach(string sub_directory in Directory.GetDirectories(directory)) {
            GetTracksFromDirectory(ref tracks, sub_directory);
        }
    }

    public static List<string> RemoveInvaidTracks(List<string> tracks) {
        for(int i = tracks.Count - 1; i >= 0; i--) {
            if(!Tools.ValidAudioFile(tracks[i]) && !Directory.Exists(tracks[i])) {
                tracks.RemoveAt(i);
            }
        }

        return tracks;
    }

    public static List<string> IncludeDirectoryTracks(List<string> tracks) {
        for(int i = tracks.Count - 1; i >= 0; i--) {
            if (Directory.Exists(tracks[i])) {
                string dir_path = tracks[i];
                tracks.RemoveAt(i);

                List<string> dir_tracks = new List<string>();
                GetTracksFromDirectory(ref dir_tracks, dir_path);
                tracks.InsertRange(i, dir_tracks);
            }
        }
        
        return tracks;
    }
}

public struct CustomInfo
{
    public string overlay_color, background_path;

    public CustomInfo() {
        overlay_color = "";
        background_path = "";
    }
}