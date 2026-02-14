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
        Globals.save_data.playlists.Remove(GetPath());
        Globals.save_data.Save();
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
                string path = songs[i];
                songs.RemoveAt(i);
                
                if (Directory.Exists(path))
                {
                    songs.InsertRange(i, GetSongsFromDirectory(path));
                }
            }
        }

        return songs;
    }

    public static List<string> GetSongsFromDirectory(string directory)
    {
        List<string> songs = new List<string>();

        foreach(string d in Directory.GetDirectories(directory))
        {
            if (Directory.Exists(d))
            {
                songs.AddRange(GetSongsFromDirectory(d));
            }
        }

        foreach(string s in Directory.GetFiles(directory))
        {
            
            if(Tools.ValidAudioFile(s))
            {
                songs.Add(s);
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