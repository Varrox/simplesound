using Godot;

public class ApplicationMetadata
{
	public bool remove_song_warning = true;
	public int blur_quality = 3;
	public bool vsync = true;

    public string appdata_location;

    public ApplicationMetadata() {}

	public void ApplySettings()
	{
		RenderingServer.GlobalShaderParameterSet("blur_quality", blur_quality);
		ProjectSettings.SetSetting("display/window/vsync/vsync_mode", vsync ? 1 : 0);
        if (!string.IsNullOrEmpty(appdata_location))
        {
            SaveSystem.USER_DATA = appdata_location;
        }
	}
}