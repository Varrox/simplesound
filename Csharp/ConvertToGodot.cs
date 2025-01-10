using Godot;

public class ConvertToGodot
{
    public static Texture2D getCover(string path)
    {
        byte[] pictureData = Metadata.GetCover(path, out string type);
        if (pictureData == null) return ImageTexture.CreateFromImage(ResourceLoader.Load<Texture2D>("res://Icons/DefaultCover.png").GetImage());
        Image image = new Image();
        switch (type)
        {
            case "image/jpeg":
                image.LoadJpgFromBuffer(pictureData);
                break;
            case "image/png":
                image.LoadPngFromBuffer(pictureData);
                break;
            case "image/webp":
                image.LoadWebpFromBuffer(pictureData);
                break;
        }

        Texture2D t = new Texture2D();
        t = ImageTexture.CreateFromImage(image);
        return t;
    }

    public static Color GetAverageColor(Texture2D texture, int samples)
    {
        Color color = new Color();
        Image image = texture.GetImage();
        int x = image.GetWidth() - 1;
        int y = image.GetHeight() - 1;

        for (int i = 0; i < samples; i++)
        {
            color += image.GetPixel(GD.RandRange(0, x), GD.RandRange(0, y));
        }

        return color / samples;
    }
}