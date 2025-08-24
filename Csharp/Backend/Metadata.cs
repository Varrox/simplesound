using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using TagLib;

public class Metadata
{
    public static Dictionary<string, string[]> TagList = new Dictionary<string, string[]>();
    public static Dictionary<string, TagLib.File> FileList = new Dictionary<string, TagLib.File>();

    public static bool InitializeFileListKey(ref string path)
    {
        if (FileList.ContainsKey(path))
            return true;
        else if (IsFileCorrupt(path))
            return false;

        TagLib.File file = TagLib.File.Create(path);
        FileList.Add(path, file);

        return true;
    }

    public static bool IsFileCorrupt(string path)
    {
        try
        {
            TagLib.File file = TagLib.File.Create(path);

            return file.PossiblyCorrupt;
        }
        catch
        {
            return true;
        }
    }

    public static void InitializeTagListKey(ref string path)
    {
        if (!TagList.ContainsKey(path))
        {
            if (InitializeFileListKey(ref path))
            {
                string comment = FileList[path].Tag.Comment;
                TagList.Add(path, comment != null ? comment.Split(';') : null);
            }
        }
    }

    public static void ResetCache()
    {
        FileList.Clear();
        TagList.Clear();
    }

    public static string GetArtist(string path)
    {
        InitializeFileListKey(ref path);
        return FileList[path].Tag.FirstPerformer ?? "Unknown Artist";
    }

    public static float GetTotalTime(string path)
    {
        InitializeFileListKey(ref path);
        return (float)FileList[path].Properties.Duration.TotalSeconds;
    }

    public static byte[] GetCover(string path, out string type)
    {
        InitializeFileListKey(ref path);
        IPicture[] pictures = FileList[path].Tag.Pictures;

        if (pictures.Length > 0)
        {
            type = pictures[0].MimeType;
            return TagLib.File.Create(path).Tag.Pictures[0].Data.Data;
        }

        type = null;
        return null;
    }

    public static void WriteToComment(string path, string comment)
    {
        InitializeFileListKey(ref path);
        FileList[path].Tag.Comment = comment;
        FileList[path].Save();
    }

    public static string GetComment(string path)
    {
        InitializeFileListKey(ref path);
        return FileList[path].Tag.Comment;
    }

    public static string[] GetLyrics(string path)
    {
        InitializeFileListKey(ref path);
        return FileList[path].Tag.Lyrics.Split('\n');
    }

    public static string GetName(string path)
    {
        InitializeFileListKey(ref path);
        return FileList[path].Tag.Title;
    }

    public static void SetData(string path, string name, string artist, string coverpath, string sharelink, bool explicitLyrics)
    {
        InitializeFileListKey(ref path);

        FileList[path].Tag.Title = name ?? FileList[path].Tag.Title;

        if (artist != null)
            FileList[path].Tag.Performers = new[] {artist};

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

            FileList[path].Tag.Pictures = new[] { picture };
        }

        List<string> tags = new List<string>();

        if(explicitLyrics)
            tags.Add("Explicit");

        if(sharelink != null || sharelink.Trim() != "")
            tags.Add($"Link {sharelink}");

        if (tags.Count > 0)
        {
            FileList[path].Tag.Comment = string.Join(';', tags);

            if (!FileList.ContainsKey(path))
                TagList.Add(path, null);
            TagList[path] = tags.ToArray();
        }

        FileList[path].Save();
    }

    public static string ImageMimeConvert(string path)
    {
        string extention = Path.GetExtension(path);
        if (extention != string.Empty)
            return $"image/{Path.GetExtension(path).Substring(1).Replace("jpg", "jpeg")}";
        return null;
    }

    public static bool IsExplicit(string path)
    {
        InitializeTagListKey(ref path);
        return Array.IndexOf(TagList[path] ?? new string[] {} , "Explicit") != -1;
    }
    
    public static string GetShareLink(string path)
    {
        InitializeTagListKey(ref path);

        for (int i = 0; i < TagList[path].Length; i++)
        {
            if(TagList[path][i].Trim().StartsWith("Link "))
            {
                GD.Print("Tag found");
                return TagList[path][i].Substring(5);
            }
        }

        return null;
    }
}