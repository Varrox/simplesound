using System;
using System.Collections.Generic;
using System.IO;
using System.Text.Json;
using Godot;
using System.Text;

public struct JsonHelper {
    public static void WriteVector2I(in Utf8JsonWriter writer, string property_name, Vector2I vector) {
        writer.WriteStartObject(property_name);
        writer.WriteNumber("x", vector.X);
        writer.WriteNumber("y", vector.Y);
        writer.WriteEndObject();
    }

    public static Vector2I GetVector2I(ref Utf8JsonReader reader) {
        Vector2I vector = new Vector2I();

        string[] x = ["X", "x"];
        string[] y = ["Y", "y"];

        while(reader.Read()) {
            if (reader.TokenType == JsonTokenType.EndObject) break;

            if (reader.TokenType == JsonTokenType.PropertyName) {
                string property_name = reader.GetString();
                reader.Read();
                
                if (x.Contains(property_name)) {
                    if (reader.TokenType == JsonTokenType.Number) vector.X = reader.GetInt32();
                }
                else if(y.Contains(property_name)) {
                    if (reader.TokenType == JsonTokenType.Number) vector.Y = reader.GetInt32();
                }
            }
        }

        return vector;
    }

    public static List<string> GetStringList(ref Utf8JsonReader reader) {
        List<string> list = new List<string>();

        while(reader.Read()) {
            if(reader.TokenType == JsonTokenType.EndArray) {
                break;
            }
            list.Add(reader.GetString() ?? "");
        }

        return list;
    }

    public static ReadOnlySpan<byte> LoadJsonBytes(string path) {
        ReadOnlySpan<byte> utf8_bom = [ 0xEF, 0xBB, 0xBF ];
        ReadOnlySpan<byte> bytes = Encoding.UTF8.GetBytes(File.ReadAllText(path, Encoding.UTF8));

        if (bytes.Length >= 3 && bytes[0] == utf8_bom[0] && bytes[1] == utf8_bom[1] && bytes[2] == utf8_bom[2]) {
            bytes = bytes[3..]; // Skip the first 3 bytes
        }

        return bytes;
    }

    public static JsonWriterOptions CreateJsonWriterOptions() {
        return new JsonWriterOptions {
            Indented = true,
            IndentSize = 4
        };
    }

    public static JsonReaderOptions CreateJsonReaderOptions() {
        return new JsonReaderOptions {
            AllowTrailingCommas = true,
            CommentHandling = JsonCommentHandling.Skip
        };
    }

    public static T LoadSettingsFromJson<T>(ref Utf8JsonReader reader) where T : ISettings, new() {
        if (reader.TokenType != JsonTokenType.StartObject)
            throw new JsonException("Expected StartObject token");

        T settings = new();

        while(reader.Read()) {
            if (reader.TokenType == JsonTokenType.EndObject) return settings;

            if (reader.TokenType == JsonTokenType.PropertyName) {
                string property_name = reader.GetString();

                reader.Read();
                
                settings.UpdateProperty(property_name, ref reader);
            }
        }

        throw new JsonException("Incomplete JSON object");
    }
}