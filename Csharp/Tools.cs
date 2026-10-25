using Godot;
using System.IO;

public class Tools {
    /// <summary>
    /// Converts a value in seconds to a time step. (example : 60 seconds -> 1:00)
    /// </summary>
    /// <param name="time">time in seconds</param>
    /// <returns>Timestamp</returns>
    public static string SecondsToTimestamp(float time) {
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
    public static string GetMediaTitle(string path) {
        return Metadata.GetName(path) ?? Path.GetFileNameWithoutExtension(path);
    }

    /// <summary>
    /// Whether a file is a valid audio file or not, and if the file exists. (supported file types are: mp3, wav, ogg)
    /// </summary>
    /// <param name="path">audio filepath</param>
    /// <returns>audio file validness</returns>
    public static bool ValidAudioFile(string path) {
        if(!File.Exists(path)) return false;

        return IsValidAudioFormat(path);
    }

    public static Texture2D GetMediaCover(string path) {
        byte[] picture_data = Metadata.GetCover(path, out string type);
        
        if (picture_data == null) return null;

        Image image = new Image();
        Error error = Error.Failed;

        switch (type) {
            case "image/jpg":
            case "image/jpeg":
                error = image.LoadJpgFromBuffer(picture_data);
                break;
            case "image/png":
                error = image.LoadPngFromBuffer(picture_data);
                break;
            case "image/webp":
                error = image.LoadWebpFromBuffer(picture_data);
                break;
            case "image/bmp":
                error = image.LoadBmpFromBuffer(picture_data);
                break;
            case "image/svg+xml":
                error = image.LoadSvgFromBuffer(picture_data);
                break;
            case "image/vnd-ms.dds":
                error = image.LoadDdsFromBuffer(picture_data);
                break;
            case "image/x-targa":
            case "image/x-tga":
                error = image.LoadTgaFromBuffer(picture_data);
                break;
        }

        return error == Error.Ok ? ImageTexture.CreateFromImage(image) : null;
    }

    /// <summary>
    /// Loads an image file into a Texture2D
    /// </summary>
    /// <param name="file_name">path to the image</param>
    /// <param name="fallback">the Texture2D to be returned if the image fails to load</param>
    /// <returns></returns>
    public static Texture2D LoadImageFromFile(string file_name) {
        if (file_name == string.Empty || file_name == null || !File.Exists(file_name)) return null;
        Image img = new Image();
        if (img.Load(file_name) == Error.Ok) return ImageTexture.CreateFromImage(img);
        return null;
    }

    public static bool IsValidAudioFormat(string path) {
        for(int i = 0; i < Constants.PLAYABLE_FORMATS.Length; i++) {
            if(path.GetExtension() == Constants.PLAYABLE_FORMATS[i]) return true;
        }

        return false;
    }

    public static string GetProperFileName(string text) {
        return text.Replace('\\', '-').Replace('/', '-').Replace(':', '-');
    }
}