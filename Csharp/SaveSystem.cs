using System;
using System.Collections.Generic;
using System.IO;
using SSLParser;

public class SaveSystem
{
	public static readonly string USER_DATA = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "simplesound");

	public static void GetSaveData(out int playlist_index, out int song_index, out float current_time, out float volume, out bool shuffled)
	{
        string playlists = Path.Combine(USER_DATA, "Playlists");
        if (!Directory.Exists(playlists))
			Directory.CreateDirectory(playlists);

        string music_files = Path.Combine(USER_DATA, "Music Folders");
        if (!Directory.Exists(music_files))
			Directory.CreateDirectory(music_files);

        string playlist_covers = Path.Combine(USER_DATA, "Playlist Covers");
        if (!Directory.Exists(playlist_covers))
			Directory.CreateDirectory(playlist_covers);

        playlist_index = 0;
        song_index = 0;
        current_time = 0;
        volume = 0;
        shuffled = false;

        string save_data = Path.Combine(USER_DATA, "savedata.txt");

        if (!File.Exists(save_data))
		{
			File.Create(save_data).Close();
			File.WriteAllText(save_data, "0\n0\n0.0\n0.0\nfalse");
        }
		else
		{
			string[] data = File.ReadAllLines(save_data);

			for(int i = 0; i < data.Length; i++)
			{
				switch(i)
				{
					case 0:
                        playlist_index = Convert.ToInt32(data[0]);

                        break;
					case 1:
                        song_index = Convert.ToInt32(data[1]);
                        break;
					case 2:
                        current_time = Convert.ToSingle(data[2]);
                        break;
					case 3:
						volume = Convert.ToSingle(data[3]);
						break;
					case 4:
                        shuffled = Convert.ToBoolean(data[4]);
                        break;
				}
			}
        }
    }

	public static void SaveData(int playlist_index, int song_index, float current_time, float volume, bool shuffled)
	{
        string save_data = Path.Combine(USER_DATA, "savedata.txt");
        File.WriteAllText(save_data, $"{playlist_index}\n{song_index}\n{current_time}\n{volume}\n{shuffled}");
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


        string newPath = Path.Combine(USER_DATA, "Playlist Covers", playlist_name + Path.GetExtension(path));
        File.Copy(path, newPath);

		return newPath;
    }
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
