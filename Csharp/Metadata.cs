using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ATL;

public class Metadata
{
    public static Dictionary<string, string[]> tag_list = new Dictionary<string, string[]>(); // File to tag
    public static Dictionary<string, Track> file_list = new Dictionary<string, Track>(); // File to taglib file

    public static bool InitializeFileListKey(in string path) {
        if (file_list.ContainsKey(path)) return true;
        else if (IsFileCorrupt(path)) return false;

        Track file = new Track(path);
        file_list.Add(path, file);
        
        return true;
    }

    public static bool IsFileCorrupt(in string path) {
        if(!File.Exists(path)) return true;
        if(new FileInfo(path).Length == 0) return true;
        try {
            Track file = new Track(path);
            if (file.Duration <= 0)
                return true;
            return false;
        }
        catch { return true; }
    }

    public static bool InitializeTagListKey(in string path) {
        if(tag_list.ContainsKey(path)) return true;
        if (!InitializeFileListKey(path)) return false;

        string comment = file_list[path].Comment;
        tag_list.Add(path, comment?.Split(';'));
        return true;
    }

    public static void ResetCache() {
        file_list.Clear();
        tag_list.Clear();
    }

    public static string GetArtist(in string path) {
        if(InitializeFileListKey(path)) return file_list[path].Artist ?? "Unknown Artist";
        return "Unknown Artist";
    }

    public static float GetTotalTime(in string path) {
        if(InitializeFileListKey(path)) return (float)file_list[path].DurationMs / 1000;
        return 0f;
    }

    public static byte[] GetCover(in string path, out string type) {
        if(InitializeFileListKey(path)) {
            var pictures = file_list[path].EmbeddedPictures;

            if (pictures.Count > 0) {
                type = pictures[0].MimeType;
                return pictures[0].PictureData;
            }
        }

        type = null;
        return null;
    }

    public static void WriteToComment(in string path, in string comment) {
        if(InitializeFileListKey(path)) {
            file_list[path].Comment = comment;
            file_list[path].Save();
        }
    }

    public static string GetComment(in string path) {
        if(InitializeFileListKey(path)) return file_list[path].Comment;
        return null;
    }

    /*public static string[] GetLyrics(in string path) {
        if(InitializeFileListKey(path)) {
            var lyrics = file_list[path].Lyrics;
            
            return null;
        }
        return null;
    }*/

    public static string GetName(in string path) {
        if(InitializeFileListKey(path)) return file_list[path].Title;
        return null;
    }

    public static void SetData(in string path, in string name, in string artist, in string cover_path, in string share_link, in bool explicit_lyrics) {
        if (!InitializeFileListKey(path)) return;

        if(name != null) file_list[path].Title = name;

        if (artist != null) file_list[path].Artist = artist;

        string file_type = ImageMimeConvert(cover_path);

        if(file_type != null) {
            byte[] data = File.ReadAllBytes(cover_path);

            PictureInfo picture = PictureInfo.fromBinaryData(data, PictureInfo.PIC_TYPE.Front);

            file_list[path].EmbeddedPictures.Clear();
            file_list[path].EmbeddedPictures.Add(picture);
        }

        List<string> tags = new List<string>();

        if(explicit_lyrics) tags.Add("Explicit");

        if(share_link != null || share_link.Trim() != "") tags.Add($"Link {share_link}");

        if (tags.Count > 0) {
            file_list[path].Comment = string.Join(';', tags);

            if (!file_list.ContainsKey(path)) tag_list.Add(path, null);
            tag_list[path] = tags.ToArray();
        }

        file_list[path].Save();
    }

    public static string ImageMimeConvert(in string path) {
        string extention = Path.GetExtension(path);
        if (extention != string.Empty) return $"image/{extention.Substring(1)}";
        return null;
    }

    public static bool IsExplicit(in string path) {
        return IsTagDefined(path, "Explicit");
    }
    
    public static string GetShareLink(in string path) {
        return GetTag(path, "Link");
    }

    public static string GetVideo(in string path) {
        return GetTag(path, "Video");
    }

    public static string GetTag(in string path, in string tag) {
        if (!InitializeTagListKey(path)) return "";
        
        if (tag_list[path] == null) return "";

        for (int i = 0; i < tag_list[path].Length; i++) {
            if (tag_list[path][i].Trim().StartsWith($"{tag} ")) {
                return tag_list[path][i].Substring(tag.Length + 1).Trim();
            }
        }
        
        return "";
    }

    public static bool IsTagDefined(in string path, in string tag) {
        if(InitializeTagListKey(path)) return Array.IndexOf(tag_list[path] ?? [] , tag) != -1;
        return false;
    }
}