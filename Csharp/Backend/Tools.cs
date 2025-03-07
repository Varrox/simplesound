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
    /// Returns the first index of where a string is inside of an array.
    /// If the string is not found, the function will return -1
    /// </summary>
    /// <param name="what">the string to be found inside of the array</param>
    /// <param name="array">array to be searched through</param>
    /// <returns>index</returns>
    public static int FindString(string what, ref string[] array)
    {
        for (int i = 0; i < array.Length; i++)
        {
            if (array[i] == what) return i;
        }
        return -1;
    }

    /// <summary>
    /// Converts a value in seconds to a time step. (example : 60 seconds -> 1:00)
    /// </summary>
    /// <param name="time">time in seconds</param>
    /// <returns>Timestamp</returns>
    public static string SecondsToTimestamp(float time)
    {
        int mins = (int)time / 60;

        string hours = (mins / 60).ToString();

        string minsmodsixty = (mins % 60).ToString();

        string minutes = (mins % 60) < 10 ? (mins < 10 ? $"{minsmodsixty}" : minsmodsixty) : minsmodsixty;

        int inttimemodsixty = (int)(time % 60);

        string seconds = inttimemodsixty < 10 ? $"0{inttimemodsixty}" : inttimemodsixty.ToString();

        return (int)(time / 3600) != 0 ? $"{hours}:{minutes}:{seconds}" : $"{minutes}:{seconds}";
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
}
