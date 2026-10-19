using System.Linq;
using System.Text.Json;
using Godot;

public class GraphicSettings : ISettings {
    public QualityLevel blur_quality;

    // Display Settings

    public bool vsync;
    public int max_fps;
    public bool reduce_fps_on_lose_focus;
    public Vector2I main_display_size;

    // SORT THESE BY AGE. DO NOT REMOVE OLD NAMES UNTIL ALPHA IS OVER, OR ALPHA v1.4.

    public static readonly string[] PROPERTY_BLUR_QUALITY = ["blur_quality"];

	public static readonly string[] PROPERTY_VSYNC = ["vsync"];
    public static readonly string[] PROPERTY_MAX_FPS = ["max_fps"];
    public static readonly string[] PROPERTY_REDUCE_FPS_ON_LOSE_FOCUS = ["reduce_fps_on_lose_focus"];
    public static readonly string[] PROPERTY_MAIN_DISPLAY_SIZE = ["main_display_size"];

    public GraphicSettings() {
        blur_quality = QualityLevel.MEDIUM;

        vsync = true;
        max_fps = 60;
        reduce_fps_on_lose_focus = true;
        main_display_size = new Vector2I((int)ProjectSettings.GetSetting("display/window/size/viewport_width"), (int)ProjectSettings.GetSetting("display/window/size/viewport_height"));
    }

    public void ApplySettings() {
        RenderingServer.GlobalShaderParameterSet("blur_quality", (int)blur_quality + 1);
        DisplayServer.WindowSetVsyncMode(vsync ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);
        Engine.MaxFps = max_fps;
    }

    public void WriteJson(in Utf8JsonWriter writer) {
        writer.WriteNumber(PROPERTY_BLUR_QUALITY[0], (int)blur_quality);

        writer.WriteBoolean(PROPERTY_VSYNC[0], vsync);
        writer.WriteNumber(PROPERTY_MAX_FPS[0], max_fps);
        writer.WriteBoolean(PROPERTY_REDUCE_FPS_ON_LOSE_FOCUS[0], reduce_fps_on_lose_focus);

        JsonHelper.WriteVector2I(writer, PROPERTY_MAIN_DISPLAY_SIZE[0], main_display_size);
    }

    public void UpdateProperty(string property_name, ref Utf8JsonReader reader) {
        if (PROPERTY_BLUR_QUALITY.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.Number) blur_quality = (QualityLevel)reader.GetInt32();
        }
        else if (PROPERTY_VSYNC.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False) vsync = reader.GetBoolean();
        }
        else if (PROPERTY_MAX_FPS.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.Number) max_fps = reader.GetInt32();
        }
        else if (PROPERTY_REDUCE_FPS_ON_LOSE_FOCUS.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.True || reader.TokenType == JsonTokenType.False) reduce_fps_on_lose_focus = reader.GetBoolean();
        }
        else if (PROPERTY_MAIN_DISPLAY_SIZE.Contains(property_name)) {
            if (reader.TokenType == JsonTokenType.StartObject) main_display_size = JsonHelper.GetVector2I(ref reader);
        }
        
        reader.Skip();
    }
}