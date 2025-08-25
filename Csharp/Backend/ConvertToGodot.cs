using Godot;
using SSLParser;
using System;
using System.IO;

public class ConvertToGodot
{
    public static Texture2D GetCover(string path)
    {
        byte[] pictureData = Metadata.GetCover(path, out string type);
        
        if (pictureData == null)
            return Globals.default_cover;

        Image image = new Image();
        Error error = Error.Failed;

        switch (type)
        {
            case "image/jpeg":
                error = image.LoadJpgFromBuffer(pictureData);
                break;
            case "image/png":
                error = image.LoadPngFromBuffer(pictureData);
                break;
            case "image/webp":
                error = image.LoadWebpFromBuffer(pictureData);
                break;
        }

        return error == Error.Ok ? ImageTexture.CreateFromImage(image) : Globals.default_cover;
    }

    public static VideoStreamTheora GetVideo(string path)
    {
        string video_path = Metadata.GetVideo(path);

        if (video_path == string.Empty)
            return null;

        if (File.Exists(video_path) && video_path.EndsWith(".ogv"))
        {
            Debug.Log("KMS");
            return Godot.ResourceLoader.Load(video_path) as VideoStreamTheora;
        }

        return null;
    }

    public static Color GetAverageColor(Texture2D texture, int samples = 6)
    {
        Image image = texture.GetImage();
        Vector2I pos = new Vector2I(image.GetWidth() - 1, image.GetHeight() - 1);

        Color color = new Color();

        for (int i = 0; i < samples; i++)
        {
            color += image.GetPixel((pos.X / samples) * i, (pos.Y / samples) * i);
        }

        return color / samples;
    }

    public static Color GetColor(string text)
    {
        if (text[0] == '#')
        {
            return Color.FromHtml(text);
        }
        else
        {
            string[] colorVals = ParsingTools.GetInParenthases(text, out string argument);
            switch(argument)
            {
                case "rgb":
                    return new Color(Convert.ToSingle(colorVals[0]), Convert.ToSingle(colorVals[1]), Convert.ToSingle(colorVals[2]), 1);
                case "rgba":
                    return new Color(Convert.ToSingle(colorVals[0]), Convert.ToSingle(colorVals[1]), Convert.ToSingle(colorVals[2]), Convert.ToSingle(colorVals[3]));
            }
            
            GD.PrintErr($"Could not parse color properly, invalid method name \'{text}\'");
            return new Color();
        }
    }

    /// <summary>
    /// Loads an image file into a Texture2D
    /// </summary>
    /// <param name="filename">path to the image</param>
    /// <param name="fallback">the Texture2D to be returned if the image fails to load</param>
    /// <returns></returns>
    public static Texture2D LoadImage(string filename, ref Texture2D fallback)
    {
        if(filename != null)
        {
            Image img = new Image();
            if (img.Load(filename) == Error.Ok) return ImageTexture.CreateFromImage(img);
        }
        return fallback;
    }

    /// <summary>
    /// Loads shader code from a .gdshader into the shader type and caches it WARNING: DO NOT USE, SHADERS AREN'T SUPPOSED TO BE CACHED LIKE THIS DOES
    /// </summary>
    /// <param name="path">the path to the .gdshader file</param>
    /// <param name="cachedShader">the path of the cached shader</param>
    /// <returns>compiled shader</returns>
    public static Shader LoadShader(string path)
    {
        Shader shader = new Shader();
        shader.Code = File.ReadAllText(path);
        shader.ResourceName = Path.GetFileNameWithoutExtension(path);

        return shader;
    }
}