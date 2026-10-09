using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class SaveSystem
{
    public static string ImportFolder(in string path) {
        // Create new folder
		string new_path = Path.Combine(Constants.USER_TRACKS, Path.GetDirectoryName(path));
        Directory.CreateDirectory(new_path);

		// Copy all audio files to the folder
		string[] tracks = Directory.GetFiles(path);

		foreach (string track in tracks) { 
			if(Tools.ValidAudioFile(track)) File.Copy(track, Path.Combine(new_path, Path.GetFileName(track)));
		}

		return new_path;
    }

	public static List<string> ImportTracks(in string[] tracks, string playlist_name, bool check_valid = true) {
		List<string> list = new List<string>();
		string new_path = Path.Combine(Constants.USER_TRACKS, playlist_name);
		Directory.CreateDirectory(new_path);

		foreach(string track in tracks) {
			if(check_valid) if (Tools.ValidAudioFile(track)) continue;

			string destination_path = Path.Combine(new_path, Path.GetFileName(track));

			File.Copy(track, destination_path);
            list.Add(destination_path);
        }

		return list;
	}

	public static string ImportCover(string path, string playlist_name, bool check_valid = true) {
		if (check_valid) if (!File.Exists(path)) return "";

        string new_path = Path.Combine(Constants.USER_PLAYLIST_COVERS, playlist_name + Path.GetExtension(path));
        File.Copy(path, new_path);

		return new_path;
    }
}

public class SaveData
{
	public int playlist_index, song_index, looked_at_playlist;
	public float time, volume;

	public bool shuffled;

	public List<string> playlists;
	
	public ApplicationSettings application_settings = new ApplicationSettings();
	public CloudSettings cloud_settings = new CloudSettings();
	public AudioSettings audio_settings = new AudioSettings();
	public GraphicSettings graphic_settings = new GraphicSettings();

    static readonly string path = Path.Combine(Constants.USER_DATA, "savedata.json");

    public void Save() {
        File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
    }

    public static SaveData GetSaveData() {
        foreach (string folder_path in new[] { Constants.USER_PLAYLISTS, Constants.USER_TRACKS, Constants.USER_PLAYLIST_COVERS }) { // Add folders
            if (!Directory.Exists(folder_path)) Directory.CreateDirectory(folder_path);
        }

        SaveData save_data;

        if (!File.Exists(path)) {
            save_data = new();
            save_data.Save();
        }
        else save_data = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(path));

        return save_data;
    }
}