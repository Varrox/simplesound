using Godot;
using SSLParser;
using System;

public class ConvertToGodot
{
    public static Texture2D getCover(string path)
    {
        byte[] pictureData = Metadata.GetCover(path, out string type);
        if (pictureData == null) return ImageTexture.CreateFromImage(ResourceLoader.Load<Texture2D>("res://Icons/DefaultCover.png").GetImage());
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
            image.Load("res://Icons/DefaultCover.png");
            return ImageTexture.CreateFromImage(image);
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
            string[] colorVals = ParsingTools.GetInParenthases(text, out string before);
            switch(before)
            {
                case "rgb":
                    return new Color(Convert.ToSingle(colorVals[0]), Convert.ToSingle(colorVals[1]), Convert.ToSingle(colorVals[2]), 1);
                case "rgba":
                    return new Color(Convert.ToSingle(colorVals[0]), Convert.ToSingle(colorVals[1]), Convert.ToSingle(colorVals[2]), Convert.ToSingle(colorVals[3]));
            }
            return new Color();
        }
    }

    public static Texture2D LoadImage(string filename, ref Texture2D fallback)
    {
        var img = new Image();
        if (img.Load(filename) == Error.Ok) return ImageTexture.CreateFromImage(img);
        else return fallback;
    }
}