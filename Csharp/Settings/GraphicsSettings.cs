using Godot;

public class GraphicSettings : ISettings
{
    public int blur_quality = 2;

    public GraphicSettings() {}

    public void ApplySettings() {
        RenderingServer.GlobalShaderParameterSet("blur_quality", blur_quality + 1);
    }
}