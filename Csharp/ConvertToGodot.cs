using Godot;

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

        if(error == Error.Ok)
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
}