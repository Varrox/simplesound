using System;
using System.Collections.Generic;
using System.IO;
using SSLParser;

public class SaveSystem
{
	public static readonly string UserData = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "simplesound");

	public static void InitData(out int playlistIndex, out int songIndex, out float currentTime, out float volume)
	{
        string appdata = UserData;
        string saveData = Path.Combine(appdata, "savedata.txt");
		string playlistSaver = Path.Combine(appdata, "savedplaylists.txt");
		string playlists = Path.Combine(appdata, "Playlists");
		string musicFiles = Path.Combine(appdata, "Music Folders");
		string playlistCovers = Path.Combine(appdata, "Playlist Covers");

        if (!Directory.Exists(playlists))
		{
			Directory.CreateDirectory(playlists);
		}

		if (!Directory.Exists(musicFiles))
		{ 
			Directory.CreateDirectory(musicFiles);
		}

		if (!Directory.Exists(playlistCovers))
		{
			Directory.CreateDirectory(playlistCovers);
		}

		if (!File.Exists(saveData))
		{
			File.Create(saveData).Close();
			File.WriteAllText(saveData, "0\n0\n0.0\n0.0");
            playlistIndex = 0;
            songIndex = 0;
			currentTime = 0;
			volume = 0;
        }
		else
		{
			string[] data = File.ReadAllLines(saveData);
			playlistIndex = Convert.ToInt32(data[0]);
			songIndex = Convert.ToInt32(data[1]);
			currentTime = Convert.ToSingle(data[2]);
			volume = Convert.ToSingle(data[3]);
		}

		if (!File.Exists(playlistSaver))
		{
			File.Create(playlistSaver).Close();
		}
	}

	public static void SaveData(int playlistIndex, int songIndex, float currentTime, float volume)
	{
        string saveData = Path.Combine(UserData, "savedata.txt");

        if (File.Exists(saveData))
        {
            StreamWriter writer = new StreamWriter(saveData);
            writer.Write($"{playlistIndex}\n{songIndex}\n{currentTime}\n{volume}");
            writer.Close();
        }
    }

	public static string[] GetAllPlaylists()
	{
		return File.ReadAllLines(Path.Combine(UserData, "savedplaylists.txt"));
	}

    public static void SaveAllPlaylists(string[] playlists)
    {
        File.WriteAllLines(Path.Combine(UserData, "savedplaylists.txt"), playlists);
    }

    public static string ImportFolder(string path)
	{
        // Create new folder
		string newPath = Path.Combine(UserData, "Music Folders", Path.GetDirectoryName(path));
        Directory.CreateDirectory(newPath);

		// Copy all music files to the folder
		string[] songs = Directory.GetFiles(path);

		foreach (string song in songs)
		{ 
			if(Tools.ValidAudioFile(song))
            {
				File.Copy(song, Path.Combine(newPath, Path.GetFileName(song)));
			}
		}

		return newPath;
    }

	public static string ImportSongs(string[] songs, string playlist_name, bool check_valid = true)
	{
		string newPath = Path.Combine(UserData, "Music Folders", playlist_name);
		Directory.CreateDirectory(newPath);

		foreach(string song in songs)
		{
			if(check_valid)
			{
				if (Tools.ValidAudioFile(song)) continue;
			}

			File.Copy(song, Path.Combine(newPath, Path.GetFileName(song)));
		}

		return newPath;
	}

	public static string ImportCover(string path, string playlist_name, bool check_valid = true)
	{
		if (check_valid)
		{
			if (!File.Exists(path)) return "";
		}


        string newPath = Path.Combine(UserData, "Playlist Covers", playlist_name + Path.GetExtension(path));
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
