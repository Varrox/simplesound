using System;
using System.Collections.Generic;
using System.IO;
using Newtonsoft.Json;

public class SaveSystem
{
	public static readonly string USER_DATA = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "simplesound");

    public static string ImportFolder(string path)
	{
        // Create new folder
		string new_path = Path.Combine(USER_DATA, "Music Folders", Path.GetDirectoryName(path));
        Directory.CreateDirectory(new_path);

		// Copy all music files to the folder
		string[] songs = Directory.GetFiles(path);

		foreach (string song in songs)
		{ 
			if(Tools.ValidAudioFile(song))
            {
				File.Copy(song, Path.Combine(new_path, Path.GetFileName(song)));
			}
		}

		return new_path;
    }

	public static List<string> ImportSongs(string[] songs, string playlist_name, bool check_valid = true)
	{
		List<string> list = new List<string>();
		string new_path = Path.Combine(USER_DATA, "Music Folders", playlist_name);
		Directory.CreateDirectory(new_path);

		foreach(string song in songs)
		{
			if(check_valid)
			{
				if (Tools.ValidAudioFile(song)) continue;
			}

			string destination_path = Path.Combine(new_path, Path.GetFileName(song));

			File.Copy(song, destination_path);
            list.Add(destination_path);
        }

		return list;
	}

	public static string ImportCover(string path, string playlist_name, bool check_valid = true)
	{
		if (check_valid)
		{
			if (!File.Exists(path)) return "";
		}


        string new_path = Path.Combine(USER_DATA, "Playlist Covers", playlist_name + Path.GetExtension(path));
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

	static readonly string path = Path.Combine(SaveSystem.USER_DATA, "savedata.json");

    public void Save()
    {
        File.WriteAllText(path, JsonConvert.SerializeObject(this, Formatting.Indented));
    }

    public static SaveData GetSaveData()
    {
        foreach (string folder_name in new[] { "Playlists", "Music Folders", "Playlist Covers" }) // Add folders
        {
            string folder_path = Path.Combine(SaveSystem.USER_DATA, folder_name);
            if (!Directory.Exists(folder_path))
                Directory.CreateDirectory(folder_path);
        }

        SaveData save_data;

        if (!File.Exists(path))
        {
            save_data = new();
            save_data.Save();
        }
        else
        {
            save_data = JsonConvert.DeserializeObject<SaveData>(File.ReadAllText(path));
        }

        return save_data;
    }
}
