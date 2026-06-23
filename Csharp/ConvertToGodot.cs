using Godot;
using System.IO;

public class ConvertToGodot
{
    public static Texture2D GetCover(string path) {
        byte[] picture_data = Metadata.GetCover(path, out string type);
        
        if (picture_data == null) return Globals.default_cover;

        Image image = new Image();
        Error error = Error.Failed;

        switch (type) {
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
        }

        return error == Error.Ok ? ImageTexture.CreateFromImage(image) : Globals.default_cover;
    }

    /// <summary>
    /// Loads an image file into a Texture2D
    /// </summary>
    /// <param name="file_name">path to the image</param>
    /// <param name="fallback">the Texture2D to be returned if the image fails to load</param>
    /// <returns></returns>
    public static Texture2D LoadImage(string file_name) {
        if (file_name == string.Empty || file_name == null) return null;
        Image img = new Image();
        if (img.Load(file_name) == Error.Ok) return ImageTexture.CreateFromImage(img);
        return null;
    }
}