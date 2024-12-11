using Godot;

public class ConvertToGodot
{
    public static ImageTexture getCover(string path)
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

        return ImageTexture.CreateFromImage(image);
    }
}