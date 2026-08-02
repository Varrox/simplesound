using Godot;

public class GraphicSettings : ISettings
{
    public int blur_quality = 2;
    public bool vsync = true;

    public GraphicSettings() {}

    public void ApplySettings() {
        RenderingServer.GlobalShaderParameterSet("blur_quality", blur_quality + 1);
        DisplayServer.WindowSetVsyncMode(vsync ? DisplayServer.VSyncMode.Enabled : DisplayServer.VSyncMode.Disabled);
    }
}