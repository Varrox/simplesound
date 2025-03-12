using System;
using System.Collections.Generic;
using System.IO;

public class Playlist
{
    public string Name, Cover, Path;
    public List<string> Songs, Folders;

    public string Artist;
    public CustomInfo customInfo;

    public PlaylistType Type = PlaylistType.Default;
    public enum PlaylistType
    {
        Default,
        Album,
        Folder
    }

    public Dictionary<string, Action<string>> ActionMapper;

    public void Save()
    {
        SaveSystem.CreatePlaylist(this);
    }

    public void DeleteFile()
    {
        File.Delete(Path);
        string playlistSaver = System.IO.Path.Combine(SaveSystem.UserData, "savedplaylists.txt");
        List<string> paths = new List<string>(File.ReadAllLines(playlistSaver));
        paths.Remove(Path);
        File.WriteAllLines(playlistSaver, paths);
    }

    public void LoadFromFolders()
    {
        foreach (string f in Folders)
        {
            Songs.AddRange(Directory.GetFiles(f));
        }
    }

    public static bool operator true(Playlist obj) => obj != null;
    public static bool operator false(Playlist obj) => obj == null;

    public Playlist(string name, string coverpath, List<string> songs)
    {
        this.Name = name;
        this.Cover = coverpath;
        this.Songs = songs;
        customInfo = new CustomInfo();

        ActionMapper = new Dictionary<string, Action<string>>()
        {
            ["Overlay-Color"] = color => customInfo.overlayColor = color,
            ["Background-Image"] = path => customInfo.backgroundPath = path,
            ["Volume"] = amount => customInfo.volume = Convert.ToSingle(amount),
            ["Speed"] = amount => customInfo.speed = Convert.ToSingle(amount),
            ["Reverb"] = amount => customInfo.reverb = Convert.ToSingle(amount),
        };
    }

    public static Playlist CreateFromFolder(string directory, string coverpath, bool sync)
    {
        Playlist playlist = new Playlist(System.IO.Path.GetDirectoryName(directory), coverpath, null);

        if (!sync)
        {
            playlist.Songs = new List<string>();
            foreach (string song in Directory.GetFiles(directory))
            {
                if (Tools.ValidAudioFile(song))
                {
                    playlist.Songs.Add(song);
                }
            }
        }
        else
        {
            playlist.Folders = new List<string>(new[] { directory });
        }

        return playlist;
    }
}

public struct CustomInfo
{
    public string overlayColor, backgroundPath;
    public bool4 overlayReactive, backgroundReactive;
    public float volume = 0, speed = 1, reverb = 0;

    public CustomInfo()
    {

    }
}

public struct UICustomization
{
    public string DefualtFont;
    public string DefualtColor;
    public string DefualtFontColor;


    public UICustomization()
    {

    }
}