using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public struct SaveData
{
	public int playlist_index, song_index, looked_at_playlist;
	public float time, volume;

	public bool shuffled;

	public List<string> playlists;
	
	public ApplicationSettings application_settings = new ApplicationSettings();
	public CloudSettings cloud_settings = new CloudSettings();
	public AudioSettings audio_settings = new AudioSettings();
	public GraphicSettings graphic_settings = new GraphicSettings();

    public SaveData() {}

    public void Save() {
        File.WriteAllText(Constants.USER_SAVEDATA, JsonConvert.SerializeObject(this, Formatting.Indented));
    }

    public static SaveData GetSaveData() {
        foreach (string folder_path in new[] { Constants.USER_PLAYLISTS, Constants.USER_TRACKS, Constants.USER_PLAYLIST_COVERS }) { // Add folders
            if (!Directory.Exists(folder_path)) Directory.CreateDirectory(folder_path);
        }

        SaveData save_data;

        if (!File.Exists(Constants.USER_SAVEDATA)) {
            save_data = new();
            save_data.Save();
        }
        else save_data = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(Constants.USER_SAVEDATA));

        return save_data;
    }


}