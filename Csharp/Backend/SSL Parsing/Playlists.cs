using System.Collections.Generic;
using System.IO;

public class Playlist
{
    public string Name, Coverpath, path;
    public List<string> songs, folders;

    public string overlayColor, backgroundPath, artist;

    public bool4 overlayReactive, backgroundReactive;

    public float volume = 0, speed = 1, reverb = 0;

    public PlaylistType type = PlaylistType.Default;
    public enum PlaylistType
    {
        Default,
        Album,
        Folder
    }

    public void Save()
    {
        SaveSystem.CreatePlaylist(this);
    }

    public void SetType(string t)
    {
        switch (t)
        {
            case "Default":
                type = PlaylistType.Default; break;
            case "Album":
                type = PlaylistType.Album; break;
            case "Folder":
                type = PlaylistType.Folder; break;
        }
    }

    public void LoadFromFolders()
    {
        foreach (string f in folders)
        {
            songs.AddRange(Directory.GetFiles(f));
        }
    }

    public static bool operator true(Playlist obj) => obj != null;
    public static bool operator false(Playlist obj) => obj == null;
}
