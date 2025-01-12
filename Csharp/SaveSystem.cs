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

	public static string ImportFolder(string path)
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
				File.Copy(song, Path.Combine(newPath, Path.GetFileName(song)));
			}
		}

		return newPath;
    }

	public static string CreatePlaylistFromFolder(string directory, string coverpath, bool sync)
	{
		Playlist playlist = new Playlist();
		playlist.Coverpath = coverpath;
		playlist.Name = Path.GetDirectoryName(directory);
		

		if(!sync)
		{
            playlist.songs = new List<string>();
            string[] songs = Directory.GetFiles(directory);
            foreach (string song in songs)
            {
                if (song.EndsWith(".mp3"))
                {
                    playlist.songs.Add(song);
                }
            }
        }
		else
		{
			playlist.folders = new List<string>();
			playlist.folders.Add(directory);
		}

		return CreatePlaylist(playlist);
    }

    public static string CreatePlaylist(Playlist playlist)
    {
        string output = "Tags\n{\n";

		if(playlist.type != Playlist.PlaylistType.Default)
		{
			output += $"\tType : {playlist.type.ToString()}\n";
		}

		if (playlist.artist != null)
		{
			output += $"\tArtist : {playlist.artist}\n";
		}

		if(playlist.overlayColor != null)
		{
            output += $"\tOverlay-Color : {playlist.overlayColor}\n";
        }

		output += "}\n\n";
        output += "Images\n{\n";

        if (playlist.Coverpath != null)
		{
			output += $"\tCover : {playlist.Coverpath}\n";
        }

		if (playlist.BackgroundPath != null)
		{
            output += $"\tBackground-Image : {playlist.BackgroundPath}\n";
        }

        output += "}\n\n";

        if (playlist.songs != null && playlist.songs.Count > 0)
		{
			output += "Songs\n{\n";

			for (int i = 0; i < playlist.songs.Count; i++)
			{
				output += $"\t{playlist.songs[i]}\n";
			}

			output += "}\n\n";
		}

		if(playlist.folders != null && playlist.folders.Count > 0)
		{
            output += "Folders\n{\n";

            for (int i = 0; i < playlist.folders.Count; i++)
            {
                output += $"\t{playlist.folders[i]}\n";
            }

            output += "}";
        }

        string path = Path.Combine(UserData, "Playlists", playlist.Name + ".txt");
		File.WriteAllText(path, output);
		return path;
    }

    public static void DeletePlaylist(Playlist playlist)
    {
        File.Delete(playlist.path);
    }

    public static Playlist LoadPlaylist(string path)
	{
		string[] lines = File.ReadAllLines(path);

		Playlist playlist = new Playlist();

		for (int i = 0; i < lines.Length; i++)
		{
			string trimmedStartLine = lines[i].Trim();

			if(trimmedStartLine.Length == 0) continue;

			if(trimmedStartLine.StartsWith("Tags"))
			{
                for (int t = i + 1; t < lines.Length; t++)
                {
                    string trimmedLine = lines[t].Trim();
                    if (trimmedLine == "}")
                    {
                        i = t;
                        break;
                    }
                    else if (trimmedLine == "{")
                    {
                        continue;
                    }

                    string[] split = trimmedLine.Split(':');
					string var = split[0];
                    string value = split[1];

					if(var.Length > 0)
					{
                        switch (var)
                        {
                            case "Type":
                                playlist.SetType(value);
                                break;
                            case "Artist":
                                playlist.artist = value;
                                break;
                            case "Overlay-Color":
                                playlist.overlayColor = value;
                                break;
                        }

                        if (trimmedLine.EndsWith("}"))
                        {
                            break;
                        }
                    }
                }
            }
			else if(trimmedStartLine.StartsWith("Images"))
			{
				for(int c = i + 1; c < lines.Length; c++)
				{
					string trimmedLine = lines[c].Split("//")[0].Trim();
                    if (trimmedLine == "}")
                    {
                        i = c;
                        break;
                    }
                    else if (trimmedLine == "{")
                    {
                        continue;
                    }

					if(trimmedLine.Length > 0)
					{
                        string[] split = SplitLine(trimmedLine);
                        string var = split[0];
                        string value = split[1];

                        if (var.Length > 0)
                        {
                            switch (var)
                            {
                                case "Cover":
                                    playlist.Coverpath = File.Exists(value) ? value : null;
                                    break;
                                case "Background-Image":
                                    playlist.BackgroundPath = File.Exists(value) ? value : null;
                                    break;
                            }

                            if (trimmedLine.EndsWith("}"))
                            {
                                break;
                            }
                        }
                    }
                }
            }
			else if(trimmedStartLine.StartsWith("Songs"))
			{
				playlist.songs = new List<string>();
				for (int s = i + 1; s < lines.Length; s++)
                {
                    string trimmedLine = lines[s].Trim();
                    if (trimmedLine == "}")
                    {
                        i = s;
                        break;
                    }
                    else if (trimmedLine == "{")
                    {
                        continue;
                    }

					if (trimmedLine.Length > 0)
					{
                        string songPath = Path.Combine(UserData, "Music Folders", trimmedLine.Split("//")[0].Trim());

                        if (songPath.EndsWith(".mp3") && File.Exists(songPath))
                        {
                            playlist.songs.Add(songPath);

                            if (trimmedLine.EndsWith("}"))
                            {
                                break;
                            }
                        }
                    }
                }
            }
			else if(trimmedStartLine.StartsWith("Folders"))
			{
                playlist.folders = new List<string>();
                for (int f = i + 1; f < lines.Length; f++)
                {
                    string trimmedLine = lines[f].Split("//")[0].Trim();
                    if (trimmedLine == "}")
                    {
                        i = f;
                        break;
                    }
                    else if (trimmedLine == "{")
                    {
                        continue;
                    }

					if (trimmedLine.Length > 0)
					{
                        if (Directory.Exists(trimmedLine))
                        {
                            playlist.folders.Add(trimmedLine);

                            if (trimmedLine.EndsWith("}"))
                            {
                                break;
                            }
                        }
                    }
                }
            }
        }

		playlist.Name = GetFileName(path);
		return playlist;
	}

	public static string[] SplitLine(string line)
	{
		line = line.Split("//")[0];
        int index = line.IndexOf(':');
        return new[] {line.Substring(0, index).Trim(), line.Substring(index + 1).Trim()};
	}
}

public class Playlist
{
	public string Name, Coverpath, path;
	public List<string> songs, folders;

	public string overlayColor, BackgroundPath, artist;

	public PlaylistType type = PlaylistType.Default;
	public enum PlaylistType
	{
		Default,
		Album,
		Folder
	}

	public void Save()
	{
		SaveSystem.CreatePlaylist(this);
	}

	public void SetType(string t)
	{
		switch(t)
		{
			case "Default":
				type = PlaylistType.Default; break;
			case "Album":
				type = PlaylistType.Album; break;
			case "Folder": 
				type = PlaylistType.Folder; break;
		}
	}

	public void LoadFromFolders()
	{
		foreach (string f in folders)
		{
			songs.AddRange(Directory.GetFiles(f));
		}
	}

    public static bool operator true(Playlist obj) => obj != null;
    public static bool operator false(Playlist obj) => obj == null;
}
