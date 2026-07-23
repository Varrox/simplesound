public struct Constants
{
    public static readonly string[] DOWNLOAD_FORMATS = ["mp3", "wav"];
    public static readonly string[] PLAYABLE_FORMATS = ["mp3", "wav", "ogg"];
    public static readonly string[] QUALITY_LEVELS = ["Low", "Medium", "High"];
    
    public const string LATEST_RELEASE = "https://api.github.com/repos/Varrox/simplesound/releases/latest";
    public const char DOT = '\u00b7';

    public const int SPECTRUM_ANALIZER_IDX = 0;
    public const int REVERB_IDX = 1;
    public const int EQ_IDX = 2;
}