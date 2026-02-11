using Godot;
using System;
using System.Collections.Generic;
using System.IO;
using TagLib;

public class Metadata
{
    public static Dictionary<string, string[]> tag_list = new Dictionary<string, string[]>(); // File to tag
    public static Dictionary<string, TagLib.File> file_list = new Dictionary<string, TagLib.File>(); // File to taglib file

    public static bool InitializeFileListKey(ref string path)
    {
        if (file_list.ContainsKey(path))
            return true;
        else if (IsFileCorrupt(path))
            return false;

        TagLib.File file = TagLib.File.Create(path);
        file_list.TryAdd(path, file);

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

    public static bool InitializeTagListKey(ref string path)
    {
        if (!tag_list.ContainsKey(path))
        {
            if (InitializeFileListKey(ref path))
            {
                string comment = file_list[path].Tag.Comment;
                tag_list.Add(path, comment?.Split(';'));
                return true;
            }
        }
        else
        {
            return true;
        }

        return false;
    }

    public static void ResetCache()
    {
        file_list.Clear();
        tag_list.Clear();
    }

    public static string GetArtist(string path)
    {
        if(InitializeFileListKey(ref path))
            return file_list[path].Tag.FirstPerformer ?? "Unknown Artist";
        return "Unknown Artist";
    }

    public static float GetTotalTime(string path)
    {
        if(InitializeFileListKey(ref path))
            return (float)file_list[path].Properties.Duration.TotalSeconds;
        return 0f;
    }

    public static byte[] GetCover(string path, out string type)
    {
        if(InitializeFileListKey(ref path))
        {
            IPicture[] pictures = file_list[path].Tag.Pictures;

            if (pictures.Length > 0)
            {
                type = pictures[0].MimeType;
                return TagLib.File.Create(path).Tag.Pictures[0].Data.Data;
            }
        }

        type = null;
        return null;
    }

    public static void WriteToComment(string path, string comment)
    {
        if(InitializeFileListKey(ref path))
        {
            file_list[path].Tag.Comment = comment;
            file_list[path].Save();
        }
    }

    public static string GetComment(string path)
    {
        if(InitializeFileListKey(ref path))
            return file_list[path].Tag.Comment;
        return null;
    }

    public static string[] GetLyrics(string path)
    {
        if(InitializeFileListKey(ref path))
            return file_list[path].Tag.Lyrics.Split('\n');
        return null;
    }

    public static string GetName(string path)
    {
        if(InitializeFileListKey(ref path))
            return file_list[path].Tag.Title;
        return null;
    }

    public static void SetData(string path, string name, string artist, string cover_path, string share_link, bool explicit_lyrics)
    {
        if (!InitializeFileListKey(ref path))
            return;

        file_list[path].Tag.Title = name ?? file_list[path].Tag.Title;

        if (artist != null)
            file_list[path].Tag.Performers = new[] {artist};

        string file_type = ImageMimeConvert(cover_path);

        if(file_type != null)
        {
            byte[] data = System.IO.File.ReadAllBytes(cover_path);

            var picture = new TagLib.Picture
            {
                Data = new ByteVector(data),
                MimeType = file_type,
                Type = TagLib.PictureType.FrontCover
            };

            file_list[path].Tag.Pictures = new[] { picture };
        }

        List<string> tags = new List<string>();

        if(explicit_lyrics)
            tags.Add("Explicit");

        if(share_link != null || share_link.Trim() != "")
            tags.Add($"Link {share_link}");

        if (tags.Count > 0)
        {
            file_list[path].Tag.Comment = string.Join(';', tags);

            if (!file_list.ContainsKey(path))
                tag_list.Add(path, null);
            tag_list[path] = tags.ToArray();
        }

        file_list[path].Save();
    }

    public static string ImageMimeConvert(string path)
    {
        string extention = Path.GetExtension(path);
        if (extention != string.Empty)
            return $"image/{extention.Substring(1).Replace("jpg", "jpeg")}";
        return null;
    }

    public static bool IsExplicit(string path)
    {
        if(InitializeTagListKey(ref path))
            return Array.IndexOf(tag_list[path] ?? new string[] {} , "Explicit") != -1;
        return false;
    }
    
    public static string GetShareLink(string path)
    {
        if(InitializeTagListKey(ref path))
        {
            if (tag_list[path] == null)
                return null;

            for (int i = 0; i < tag_list[path].Length; i++)
            {
                if (tag_list[path][i].Trim().StartsWith("Link "))
                {
                    return tag_list[path][i].Substring(5).Trim();
                }
            }
        }

        return null;
    }

    public static string GetVideo(string path)
    {
        if (InitializeTagListKey(ref path))
        {
            if (tag_list[path] == null)
                return "";

            for (int i = 0; i < tag_list[path].Length; i++)
            {
                if (tag_list[path][i].Trim().StartsWith("Video "))
                {
                    return tag_list[path][i].Substring(6).Trim();
                }
            }
        }
        
        return "";
    }
}