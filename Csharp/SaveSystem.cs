using System.Collections.Generic;
using System.IO;

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