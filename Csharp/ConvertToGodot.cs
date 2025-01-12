using Godot;
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
        if (text.StartsWith("rgb"))
        {
            bool alpha = text[3] == 'a';

            string[] colorVals = text.Substring(4 + (alpha ? 1 : 0), text.Length - 7 + (alpha ? 1 : 0)).Split(',');

            bool byteOverFloat = false;

            for (int i = 0; i < colorVals.Length; i++)
            {
                if(!colorVals[i].Contains('.') && colorVals[i] != "1")
                {
                    byteOverFloat = true;
                    break;
                }
            }

            if (alpha)
            {
                return new Color(Convert.ToSingle(colorVals[0]), Convert.ToSingle(colorVals[1]), Convert.ToSingle(colorVals[2]), Convert.ToSingle(colorVals[3]));
            }
            else
            {
                return new Color(Convert.ToSingle(colorVals[0]), Convert.ToSingle(colorVals[1]), Convert.ToSingle(colorVals[2])) / (byteOverFloat ? 256 : 1);
            }
        }
        else if (text[0] == '#')
        {
            return Color.FromHtml(text);
        }
        else
        {
            return new Color(0, 0, 0, 0);
        }
    }

    public static Texture2D LoadImage(string filename, ref Texture2D fallback)
    {
        var img = new Image();
        if (img.Load(filename) == Error.Ok) return ImageTexture.CreateFromImage(img);
        else return fallback;
    }
}