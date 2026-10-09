using System.IO;
using System;

public struct Constants
{
    public static readonly string[] DOWNLOAD_FORMATS = ["mp3", "wav"];
    public static readonly string[] PLAYABLE_FORMATS = ["mp3", "wav", "ogg"];
    
    public const string LATEST_RELEASE = "https://api.github.com/repos/Varrox/simplesound/releases/latest";
    public const char DOT = '\u00b7';

    public const int SPECTRUM_ANALIZER_IDX = 0;
    public const int REVERB_IDX = 1;
    public const int EQ_IDX = 2;

    public static readonly string USER_DATA = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "simplesound");

    // Folders

    public static readonly string USER_PLAYLISTS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "simplesound", "playlists");
    public static readonly string USER_TRACKS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "simplesound", "tracks");
    public static readonly string USER_PLAYLIST_COVERS = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "simplesound", "data", "playlist_covers");

    // Files

    public static readonly string USER_SAVEDATA = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "savedata.json");
}