using Godot;

public class ApplicationSettings
{
	public bool remove_song_warning = true;
	public int blur_quality = 3;

    public string ytdlp_location = "";

    public ApplicationSettings() {}

	public void ApplySettings()
	{
		RenderingServer.GlobalShaderParameterSet("blur_quality", blur_quality);
	}
}