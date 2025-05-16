using Godot;
using SSLParser;
using System;
using System.IO;

public class ConvertToGodot
{
    public static Texture2D GetCover(string path, out bool failed)
    {
        byte[] pictureData = Metadata.GetCover(path, out string type);
        if (pictureData == null)
        {
            failed = true;
            return Globals.default_cover;
        }

        failed = false;

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

        if (error == Error.Ok)
        {
            Texture2D t = new Texture2D();
            return ImageTexture.CreateFromImage(image);
        }
        else
        {
            return Globals.default_cover;
        }
    }

    public static Color GetAverageColor(Texture2D texture, int samples)
    {
        Color color = new Color();
        Image image = texture.GetImage();
        int x = image.GetWidth() - 1;
        int y = image.GetHeight() - 1;

        for (int i = 0; i < samples; i++)
        {
            color += image.GetPixel((x / samples) * i, (y / samples) * i);
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
        Image img = new Image();
        if (img.Load(filename) == Error.Ok) return ImageTexture.CreateFromImage(img);
        else return fallback;
    }

    /// <summary>
    /// Loads shader code from a .gdshader into the shader type and caches it
    /// </summary>
    /// <param name="path">the path to the .gdshader file</param>
    /// <param name="cachedShader">the path of the cached shader</param>
    /// <returns>compiled shader</returns>
    public static Shader LoadShader(string path, out string cachedShader)
    {
        Shader shader = new Shader();
        shader.Code = File.ReadAllText(path);
        shader.ResourceName = Path.GetFileNameWithoutExtension(path);
        string spath = Path.Combine(SaveSystem.UserData, "Cached Shaders");
        if(ResourceSaver.Save(shader, spath) != Error.Ok)
        {
            Debug.ErrorLog($"Shader from \'{path}\' failed to cache");
            cachedShader = null;
        }
        else
            cachedShader = Path.Combine(spath, Path.GetFileNameWithoutExtension(path));

        return shader;
    }
}