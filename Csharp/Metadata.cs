using TagLib;

public class Metadata
{
    public static string GetArtist(string path)
    {
        return TagLib.File.Create(path).Tag.FirstPerformer ?? "Unknown Artist";
    }

    public static byte[] GetCover(string path, out string type)
    {
        Tag file = TagLib.File.Create(path).Tag;
        if (file.Pictures.Length > 0)
        {
            type = file.Pictures[0].MimeType;
            return TagLib.File.Create(path).Tag.Pictures[0].Data.Data;
        }
        else
        {
            type = null;
            return null;
        }
    }

    public static void WriteToComment(string path, string comment)
    {
        var file = TagLib.File.Create(path);
        file.Tag.Comment = comment;
        file.Save();
    }

    public static string GetComment(string path)
    {
        return TagLib.File.Create(path).Tag.Comment;
    }

    public static string GetName(string path)
    {
        return TagLib.File.Create(path).Tag.Title;
    }

    public static void SetData(string path, string name, string artist, string coverpath)
    {
        var file = TagLib.File.Create(path);

        file.Tag.Title = name ?? file.Tag.Title;

        if (artist != null)
        {
            file.Tag.Performers = new[] { artist ?? GetArtist(path) };
        }

        string fileType = ImageMimeConvert(coverpath);

        if(fileType != null)
        {
            byte[] data = System.IO.File.ReadAllBytes(coverpath);

            var picture = new TagLib.Picture
            {
                Data = new ByteVector(data),
                MimeType = fileType,
                Type = TagLib.PictureType.FrontCover
            };
            file.Tag.Pictures = new[] { picture };
        }

        file.Save();
    }

    public static string ImageMimeConvert(string path)
    {
        if(path.ToLower().EndsWith(".png"))
        {
            return "image/png";
        }
        else if(path.ToLower().EndsWith(".jpg") || path.ToLower().EndsWith(".jpeg"))
        {
            return "image/jpeg";
        }
        else if(path.ToLower().EndsWith(".webp"))
        {
            return "image/webp";
        }

        return null;
    }
}
