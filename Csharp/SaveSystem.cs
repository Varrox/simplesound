using System;
using System.Collections.Generic;
using System.IO;

public class SaveSystem
{
	public static string UserData
	{
		get
		{
			return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData), "simplesound");
        }
	}

	public static string GetName(string songPath)
	{
		return Metadata.GetName(songPath) ?? GetFileName(songPath);
	}

	public static string GetFileName(string path)
	{
        return Path.GetFileNameWithoutExtension(path);
    }

	public static string GetTimeFromSeconds(float time)
	{
		int mins = (int)time / 60;

		string hours = (mins/60).ToString();

		string minsmodsixty = (mins%60).ToString();

		string minutes = (mins%60) < 10 ? (mins < 10 ? $"{minsmodsixty}" : minsmodsixty) : minsmodsixty;

		int inttimemodsixty = (int)(time % 60);

        string seconds = inttimemodsixty < 10 ? $"0{inttimemodsixty}" : inttimemodsixty.ToString();

		return (int)(time / 3600) != 0 ? $"{hours}:{minutes}:{seconds}" : $"{minutes}:{seconds}";
	}

	public static void SaveSong(string songpath, int index, Playlist playlist)
	{
		if(!songpath.EndsWith(".mp3")) return; // return if file isn't correct type
        playlist.songs.Insert(index, songpath);
        playlist.Save();
	}

	public static Playlist CreatePlaylist(string name, string saveFolder, string coverpath, string playlistSaver, int index)
	{

		string path = Path.Combine(UserData, "Playlists", name + ".txt");

        if (!File.Exists(path))
		{
			File.Create(path);
			StreamWriter writer = new StreamWriter(path);
			writer.WriteLine(coverpath ?? "null");
		}

		List<string> playlists = new List<string>(File.ReadAllLines(playlistSaver));
		playlists.Insert(index, path);
		File.WriteAllText(playlistSaver, $"{string.Join("\n", playlists)}");

        return new Playlist { path = path, Coverpath = coverpath, Name = name };
    }

	public static void DeletePlaylist(Playlist playlist)
	{
		File.Delete(playlist.path);
	}

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

	public static Playlist LoadPlaylist(string playlist) // both loads and tests playlist
	{
        if(!File.Exists(playlist)) return null;
		Playlist pl = new Playlist();
        List<string> songpaths = new List<string>(File.ReadAllLines(playlist));
        pl.Coverpath = !File.Exists(songpaths[0]) || songpaths[0].Trim() == "null" ? "" : songpaths[0];
        songpaths.RemoveAt(0);

        for (int i = 0; i < songpaths.Count; i++)
        {
			songpaths[i] = songpaths[i].Trim();
            if (!songpaths[i].EndsWith(".mp3") || !File.Exists(songpaths[i]))
			{
				songpaths.RemoveAt(i);
			}
        }

        pl.songs = songpaths;
        pl.Name = GetFileName(playlist);
        pl.path = playlist;
		pl.Save();
		return pl;
    }

	public static void GetPlaylistAttributes(string filepath, out string name, out string coverpath, out int songcount)
	{
        string[] songpaths = File.ReadAllLines(filepath);
        coverpath = !File.Exists(songpaths[0]) || songpaths[0].Trim() == "null" ? null : songpaths[0];
		songcount = songpaths.Length - 1;

        for (int i = 1; i < songpaths.Length; i++)
        {
            if (!File.Exists(songpaths[i]))
            {
				songcount--;
            }
        }

		name = GetFileName(filepath);
    }

	public static string[] GetAllPlaylists()
	{
		return File.ReadAllLines(Path.Combine(UserData, "savedplaylists.txt"));
	}

	public static void ImportFolder(string path)
	{
        // Create new folder
		string newPath = Path.Combine(UserData, "Music Folders", GetName(path));
        Directory.CreateDirectory(newPath);

		// Copy all music files to the folder
		string[] songs = Directory.GetFiles(path);
		foreach (string song in songs)
		{ 
			if(song.EndsWith(".mp3"))
			{
				File.Copy(song, Path.Combine(newPath, GetFileName(song) + ".mp3"));
			}
		}
    }
}

public class Playlist
{
	public string Name, Coverpath, path;
	public List<string> songs;

	public void Save()
	{
        File.WriteAllText(path, $"{Coverpath ?? "null"}\n{string.Join("\n", songs)}");
	}

    public static bool operator true(Playlist obj) => obj != null;
    public static bool operator false(Playlist obj) => obj == null;
}
