using Godot;

public class GraphicSettings : ISettings
{
    public QualityLevel blur_quality = QualityLevel.MEDIUM;

    // Display Settings

    public bool vsync = true;
    public int max_fps = 60;
    public bool reduce_fps_on_lose_focus = true;

    public GraphicSettings() {}

    public void ApplySettings() {
        RenderingServer.GlobalShaderParameterSet("blur_quality", (int)blur_quality + 1);
        DisplayServer.WindowSetVsyncMode(vsync ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);
        Engine.MaxFps = max_fps;
    }
}