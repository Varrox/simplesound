using Godot;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

public class Playlist
{
    public string name, cover;
    public List<string> songs;

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

    public Playlist(string name, string coverpath, List<string> songs)
    {
        this.name = name;
        this.cover = coverpath;
        this.songs = ProcessSongs(songs);
        custom_info = new CustomInfo();
    }

    public static Playlist CreateFromFile(string path)
    {
        Playlist playlist = JsonConvert.DeserializeObject<Playlist>(File.ReadAllText(path));
        playlist.songs = ProcessSongs(playlist.songs);
        return playlist;
    }

    public static List<string> ProcessSongs(List<string> songs)
    {
        for(int i = 0; i < songs.Count; i++)
        {
            if(!File.Exists(songs[i]))
            {
                string directory = songs[i];
                songs.RemoveAt(i);
                if (Directory.Exists(directory))
                {
                    string[] files = Directory.GetFiles(directory);
                    foreach(string f in files)
                    {
                        if(Tools.ValidAudioFile(f))
                        {
                            songs.Insert(i, f);
                        }
                    }
                }
            }
        }

        return songs;
    }

    [JsonConstructor] public Playlist()
    {
        
    }
}

public struct CustomInfo
{
    public string overlay_color, background_path;
}

public struct UICustomization
{
    public string default_font;
    public string default_color;
    public string default_font_color;
}