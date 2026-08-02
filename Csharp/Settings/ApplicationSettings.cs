using Godot;

public class ApplicationSettings : ISettings
{
	public bool remove_song_warning = true;

	// Downloader

    public string ytdlp_location = "";
	public DownloadFormat download_format = 0;

    public ApplicationSettings() {}

	public void ApplySettings() {
        
    }
}