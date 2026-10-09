using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

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