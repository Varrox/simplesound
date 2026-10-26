using System.Text.Json;
using System.Collections.Generic;
using System.IO;
using System;
using System.Text;
using Godot;
using System.Linq;

public class Playlist {
    public string name;
    public string artist;

    public string cover {
        set {
            field = value;
            _cover_texture = Tools.LoadImageFromFile(value);
        }
        get;
    }

    public PlaylistType type;

    public List<string> tracks { get { return _tracks; } set {
            _tracks_raw = new(value);
            _tracks = ProcessTracks(value);
        } 
    }

    public List<string> tracks_raw { get { return _tracks_raw; } }

    public string path {
        get {
            if(_path == null) { _path = Path.Combine(Constants.USER_PLAYLISTS, $"{Tools.GetProperFileName(name)}.json"); }
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
    }

    public int FindRawTrackIndex(int track_index) {
        string track_path = _tracks[track_index];

        for(int i = 0; i < _tracks_raw.Count; i++) {
            if(Directory.Exists(_tracks_raw[i]) && track_path.Contains(_tracks_raw[i])) {
                List<string> directory_tracks = new List<string>();
                GetTracksFromDirectory(ref directory_tracks, _tracks_raw[i]);

                track_index -= directory_tracks.Count;
            }
            else {
                track_index--;
            }
            if (track_index <= 0) {
                return i;
            }
        }

        return -1;
    }

    public void AddTrack(string track_path) {
        if(!Tools.ValidAudioFile(track_path)) {
            return;
        }

        _tracks.Add(track_path);

        if (_tracks_raw.Contains($"!{track_path}")) {
            _tracks_raw.Add($"+{track_path}"); // Add append
        }
        else {
            _tracks_raw.Add(track_path);
        }
    }

    public void RemoveTrack(int track_index) {
        string track_path = _tracks[track_index];
        int raw_index = FindRawTrackIndex(track_index);

        if(!Directory.Exists(_tracks_raw[raw_index])) {
            _tracks_raw.RemoveAt(raw_index);
        }
        else {
            _tracks_raw.Insert(raw_index + 1, $"!{track_path}");
        }

        _tracks.RemoveAt(track_index);
    }

    public void DeleteTrack(int track_index) {
        string track_path = _tracks[track_index];

        if (_tracks_raw.Contains(track_path)) { // If it contains it, remove it.
            int index = FindRawTrackIndex(track_index);

            if(!Directory.Exists(_tracks_raw[index])) {
                _tracks_raw.RemoveAt(index);
            }
        }

        _tracks.RemoveAt(track_index);

        File.Delete(track_path);
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
    public string Save(string save_path = null) {
        File.WriteAllBytes(save_path ?? path, CreateJson());

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

        ApplicationManager.save_data.playlists.Remove(path);

        ApplicationManager.save_data.Save();
    }

    public static void GetTracksFromDirectory(ref List<string> tracks, string directory) {
        foreach(string file in Directory.GetFiles(directory)) {
            if(Tools.ValidAudioFile(file)) tracks.Add(file);
        }

        foreach(string sub_directory in Directory.GetDirectories(directory)) {
            GetTracksFromDirectory(ref tracks, sub_directory);
        }
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

    public static List<string> ProcessTracks(List<string> tracks) {
        List<string> exclude = new List<string>();
        Dictionary<int, string> insertions = new Dictionary<int, string>();

        for(int i = tracks.Count - 1; i >= 0; i--) {
            if(tracks[i].StartsWith("!")) {
                exclude.Add(tracks[i].Substring(1));
                tracks.RemoveAt(i);
                continue;
            }
            else if(tracks[i].StartsWith("+")) {
                insertions.Add(i, tracks[i].Substring(1));
                tracks.RemoveAt(i);
                continue;
            }

            if (Directory.Exists(tracks[i])) {
                string dir_path = tracks[i];
                tracks.RemoveAt(i);

                List<string> dir_tracks = new List<string>();
                GetTracksFromDirectory(ref dir_tracks, dir_path);
                tracks.InsertRange(i, dir_tracks);
            }
            else if(!Tools.ValidAudioFile(tracks[i])) { // Not an audio file.
                tracks.RemoveAt(i);
            }
        }

        for(int i = 0; i < exclude.Count; i++) { // Remove exclusions.
            tracks.RemoveAll(t => t.Equals(exclude[i]));
        }

        foreach(int key in insertions.Keys) {
            tracks.Insert(key, insertions[key]);
        }

        return tracks;
    }
}