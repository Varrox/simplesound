using Godot;
using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

public class Playlist
{
    public string name, cover;
    public List<string> songs;

    public string GetPath() {
        return path ?? System.IO.Path.Combine(Constants.USER_PLAYLISTS, $"{name.Replace('\\', '-').Replace('/', '-').Replace(':', '-')}.json");
    }

    private string path;

    public string artist;
    public CustomInfo custom_info;

    public PlaylistType type = PlaylistType.Default;

    public enum PlaylistType {
        Default,
        Album
    }

    /// <summary>
    /// Saves to path
    /// </summary>
    /// <returns></returns>
    public string Save() {
        File.WriteAllText(GetPath(), JsonConvert.SerializeObject(this, Formatting.Indented));
        return GetPath();
    }

    public void DeleteFile() {
        File.Delete(GetPath());
        Globals.save_data.playlists.Remove(GetPath());
        Globals.save_data.Save();
    }

    public Playlist(string name, string cover, List<string> tracks) {
        this.name = name;
        this.cover = cover;
        this.songs = ProcessTracks(tracks);
        custom_info = new CustomInfo();
    }

    public static Playlist LoadFromFile(string path) {
        Playlist playlist = JsonConvert.DeserializeObject<Playlist>(File.ReadAllText(path));
        playlist.songs = ProcessTracks(playlist.songs);
        return playlist;
    }

    public static List<string> ProcessTracks(List<string> tracks) {
        for(int i = 0; i < tracks.Count; i++) {
            if(!File.Exists(tracks[i])) {
                string path = tracks[i];
                tracks.RemoveAt(i);
                
                if (Directory.Exists(path)) tracks.InsertRange(i, GetTracksFromDirectory(path));
            }
        }

        return tracks;
    }

    public static List<string> GetTracksFromDirectory(string directory) {
        List<string> tracks = new List<string>();

        foreach(string dir in Directory.GetDirectories(directory)) {
            if (Directory.Exists(dir)) tracks.AddRange(GetTracksFromDirectory(dir));
        }

        foreach(string file in Directory.GetFiles(directory)) {
            if(Tools.ValidAudioFile(file)) tracks.Add(file);
        }

        return tracks;
    }

    [JsonConstructor] public Playlist() {
        
    }
}

public struct CustomInfo
{
    public string overlay_color, background_path;
}