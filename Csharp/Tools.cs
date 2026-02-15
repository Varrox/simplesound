using Godot;
using System.IO;

public class Tools
{
    /// <summary>
    /// Converts a value in seconds to a time step. (example : 60 seconds -> 1:00)
    /// </summary>
    /// <param name="time">time in seconds</param>
    /// <returns>Timestamp</returns>
    public static string SecondsToTimestamp(float time)
    {
        int total_seconds = Mathf.RoundToInt(time);
        int mins = total_seconds / 60;

        return (total_seconds / 3600 != 0 ? $"{mins / 60}:" : "") + $"{mins % 60}:{total_seconds % 60:D2}";
    }

    /// <summary>
    /// Returns the title of a media file (mp3, wav, ogg, etc.)
    /// If there is no title, the function will return the filename.
    /// </summary>
    /// <param name="path">the media path</param>
    /// <returns>Media title</returns>
    public static string GetMediaTitle(string path)
    {
        return Metadata.GetName(path) ?? Path.GetFileNameWithoutExtension(path);
    }

    /// <summary>
    /// Whether a file is a valid audio file or not, and if the file exists. (supported file types are: mp3, wav, ogg)
    /// </summary>
    /// <param name="path">audio filepath</param>
    /// <returns>audio file validness</returns>
    public static bool ValidAudioFile(string path)
    {
        if(!File.Exists(path))
            return false;

        for(int i = 0; i < Constants.playable_formats.Length; i++)
        {
            if(path.EndsWith($".{Constants.playable_formats[i]}"))
                return true;
        }

        return false;
    }
}
