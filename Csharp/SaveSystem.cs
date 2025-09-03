using System;
using System.Collections.Generic;
using System.IO;
using SSLParser;

public class SaveSystem
{
	public static readonly string USER_DATA = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "simplesound");

	public static SaveData GetSaveData()
	{
		foreach(string folder_name in new[] { "Playlists" , "Music Folders" , "Playlist Covers" }) // Add folders
		{
			string path = Path.Combine(USER_DATA, folder_name);
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);
        }

        string save_data_path = Path.Combine(USER_DATA, "savedata.ssl");

		SaveData save_data;

        if (!File.Exists(save_data_path))
		{
			save_data = new();
			save_data.Save();
        }
		else
		{
            MainParser.ParseFile(save_data_path, out save_data, out List<string> tags);
        }

        return save_data;
    }

	public static string[] GetAllPlaylists()
	{
        string playlist_save = Path.Combine(USER_DATA, "savedplaylists.txt");
        return File.Exists(playlist_save) ? File.ReadAllLines(playlist_save) : null;
	}

    public static void SaveAllPlaylists(string[] playlists)
    {
        File.WriteAllLines(Path.Combine(USER_DATA, "savedplaylists.txt"), playlists);
    }

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
	public int playlist_index, song_index;
	public float time, volume;
	public bool shuffled;

	static string path = Path.Combine(SaveSystem.USER_DATA, "savedata.ssl");

	public void Save() => File.WriteAllText(path, MainParser.StringifyObject(this));
}

public struct bool4
{
    public bool r, g, b, a;
    
    public bool4(bool r, bool g, bool b, bool a)
    {
        this.r = r;
        this.g = g;
        this.b = b;
        this.a = a;
    }
}
