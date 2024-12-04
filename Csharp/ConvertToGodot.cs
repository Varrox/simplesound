using Godot;

public class ConvertToGodot
{
    public static ImageTexture getCover(byte[] pictureData, string type)
    {
        if (pictureData == null) return null;
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
