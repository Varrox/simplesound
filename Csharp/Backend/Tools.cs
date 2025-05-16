using Godot;
using System.IO;

public class Tools
{
    /// <summary>
    /// Creates an array with a length of an input array plus 1.
    /// And copies the old array over, and sets the last index of the array to be and input item.
    /// Inefficiently adding an item to an array
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="array">base array</param>
    /// <param name="item">the item to be added</param>
    public static void AddToArray<T>(ref T[] array, T item)
    {
        T[] newArray = new T[array.Length + 1];
        array.CopyTo(newArray, 0);
        newArray[array.Length] = item;
        array = newArray;
    }

    /// <summary>
    /// Converts a value in seconds to a time step. (example : 60 seconds -> 1:00)
    /// </summary>
    /// <param name="time">time in seconds</param>
    /// <returns>Timestamp</returns>
    public static string SecondsToTimestamp(float time)
    {
        int totalSeconds = Mathf.RoundToInt(time);
        int mins = totalSeconds / 60;

        return (totalSeconds / 3600 != 0 ? $"{mins / 60}:" : "") + $"{mins % 60}:{totalSeconds % 60:D2}";
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
        return (path.EndsWith(".mp3") || path.EndsWith(".wav") || path.EndsWith(".ogg")) && File.Exists(path);
    }
}
