using Godot;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

public class Playlist
{
    public string name, cover;
    public List<string> songs, folders;

    public string GetPath()
    {
        return path != null ? path : System.IO.Path.Combine(SaveSystem.USER_DATA, "Playlists", $"{name.Replace('\\', '-').Replace('/', '-').Replace(':', '-')}.json");
    }

    private string path;

    public string artist;
    public CustomInfo custom_info;

    public PlaylistType type = PlaylistType.Default;
    public enum PlaylistType
    {
        Default,
        Album
    }

    /// <summary>
    /// Saves to path
    /// </summary>
    /// <returns></returns>
    public string Save()
    {
        File.WriteAllText(GetPath(), JsonConvert.SerializeObject(this, Formatting.Indented));
        return GetPath();
    }

    public void DeleteFile()
    {
        File.Delete(GetPath());
        string playlistSaver = System.IO.Path.Combine(SaveSystem.USER_DATA, "savedplaylists.txt");
        List<string> paths = new List<string>(File.ReadAllLines(playlistSaver));
        paths.Remove(GetPath());
        File.WriteAllLines(playlistSaver, paths);
    }

    public void LoadFromFolders()
    {
        foreach (string f in folders)
        {
            songs.AddRange(Directory.GetFiles(f));
        }
    }

    public Playlist(string name, string coverpath, List<string> songs)
    {
        this.name = name;
        this.cover = coverpath;
        this.songs = songs;
        custom_info = new CustomInfo();
    }

    public static Playlist CreateFromFile(string path)
    {
        GD.Print(path);
        return JsonConvert.DeserializeObject<Playlist>(File.ReadAllText(path));
    }

    [JsonConstructor] public Playlist()
    {
        
    }

    public static Playlist CreateFromFolder(string directory, string coverpath, bool sync)
    {
        Playlist playlist = new Playlist(System.IO.Path.GetDirectoryName(directory), coverpath, null);

        if (!sync)
        {
            playlist.songs = new List<string>();
            foreach (string song in Directory.GetFiles(directory))
            {
                if (Tools.ValidAudioFile(song))
                {
                    playlist.songs.Add(song);
                }
            }
        }
        else
        {
            playlist.folders = new List<string>(new[] { directory });
        }

        return playlist;
    }
}

public struct CustomInfo
{
    public string overlay_color, background_path;
    public float volume = 0, speed = 1, reverb = 0;

    public CustomInfo()
    {

    }
}

public struct UICustomization
{
    public string default_font;
    public string default_color;
    public string default_font_color;

    public UICustomization()
    {

    }
}