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

    public static void DeletePlaylist(Playlist playlist)
    {
        File.Delete(playlist.Path);
        string playlistSaver = Path.Combine(UserData, "savedplaylists.txt");
        List<string> paths = new List<string>(File.ReadAllLines(playlistSaver));
        paths.Remove(playlist.Path);
        File.WriteAllLines(playlistSaver, paths);
    }

    // EVERYTHING BELOW NEEDS TO BE PUT INTO THE PARSER

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

		if (playlist.backgroundPath != null)
		{
            output += $"\tBackground-Image : {playlist.backgroundPath}\n";
        }

        output += "}\n\n";

        if (playlist.Songs != null && playlist.Songs.Count > 0)
		{
			output += "Songs\n{\n";

			for (int i = 0; i < playlist.Songs.Count; i++)
			{
				output += $"\t{playlist.Songs[i]}\n";
			}

			output += "}\n\n";
		}

		if(playlist.Folders != null && playlist.Folders.Count > 0)
		{
            output += "Folders\n{\n";

            for (int i = 0; i < playlist.Folders.Count; i++)
            {
                output += $"\t{playlist.Folders[i]}\n";
            }

            output += "}";
        }

        string path = Path.Combine(UserData, "Playlists", $"{playlist.Name}.ssl");
		File.WriteAllText(path, output);
		return path;
    }

    public static Playlist LoadPlaylist(string path)
	{
		string[] lines = File.ReadAllLines(path);

		Playlist playlist = new Playlist(Path.GetFileNameWithoutExtension(path), null, null);

		for (int i = 0; i < lines.Length; i++)
		{
			string trimmedStartLine = lines[i].Trim();

			if(trimmedStartLine.Length == 0) continue;

			if(trimmedStartLine.StartsWith("Tags"))
			{
                for (int t = i + 1; t < lines.Length; t++)
                {
                    string trimmedLine = lines[t].Split("//")[0].Trim();
                    if (trimmedLine == "}")
                    {
                        i = t;
                        break;
                    }
                    else if (trimmedLine == "{")
                    {
                        continue;
                    }

                    if (trimmedLine.Length > 0)
					{
                        string[] split = SplitLine(trimmedLine);
                        string var = split[0];
                        string value = split[1];

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
                            i = t;
                            break;
                        }
                    }
                }
            }
			else if(trimmedStartLine.StartsWith("Sound"))
			{
                for (int s = i + 1; s < lines.Length; s++)
                {
                    string trimmedLine = lines[s].Split("//")[0].Trim();
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
                        string[] split = SplitLine(trimmedLine);
                        string var = split[0];
                        string value = split[1];

                        switch (var)
                        {
                            case "Volume-Reactive":
                                string[] vars = value.Split(',');
                                foreach (string var2 in vars)
                                {
                                    string[] colors = ParsingTools.GetInParenthases(var2, out string selectedVar);
                                    bool4 color = new bool4(colors[0].Contains('r'), colors[0].Contains('g'), colors[0].Contains('b'), colors[0].Contains('a'));
                                    switch (selectedVar)
                                    { 
                                        case "Overlay-Color":
                                            playlist.overlayReactive = color;
                                            break;
                                        case "Background-Image":
                                            playlist.backgroundReactive = color;
                                            break;
                                    }
                                }
                                break;
                            case "Volume":
                                playlist.volume = Convert.ToSingle(value);
                                break;
                            case "Speed":
                                playlist.speed = Convert.ToSingle(value);
                                break;
                            case "Reverb":
                                playlist.reverb = Convert.ToSingle(value);
                                break;
                        }

                        if (trimmedLine.EndsWith("}"))
                        {
                            i = s;
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
                                    playlist.backgroundPath = File.Exists(value) ? value : null;
                                    break;
                            }

                            if (trimmedLine.EndsWith("}"))
                            {
                                i = c;
                                break;
                            }
                        }
                    }
                }
            }
			else if(trimmedStartLine.StartsWith("Songs"))
			{
				playlist.Songs = new List<string>();
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

                        if (Tools.ValidAudioFile(songPath))
                        {
                            playlist.Songs.Add(songPath);

                            if (trimmedLine.EndsWith("}"))
                            {
                                i = s;
                                break;
                            }
                        }
                    }
                }
            }
			else if(trimmedStartLine.StartsWith("Folders"))
			{
                playlist.Folders = new List<string>();
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
                            playlist.Folders.Add(trimmedLine);

                            if (trimmedLine.EndsWith("}"))
                            {
                                i = f;
                                break;
                            }
                        }
                    }
                }
            }
        }

		return playlist;
	}

    public static string[] SplitLine(string line)
	{
		line = line.Split("//")[0];
        int index = line.IndexOf(':');
        return new[] {line.Substring(0, index).Trim(), line.Substring(index + 1).Trim()};
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
