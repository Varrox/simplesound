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
}