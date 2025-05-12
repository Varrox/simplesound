using Godot;
using System.Collections.Generic;

public partial class CreatePlaylist : EditorWindow
{
	[Export] TextEdit playlist_name;

	[Export] Button open_cover;
	[Export] FileDialog playlist_cover;
	[Export] PathDisplay coverdisplay;

	[Export] Button add_songs;
	[Export] FileDialog open_songs;
	[Export] PackedScene song_display;
	[Export] Control song_display_container;

	List<string> songs;

	public override void _Ready()
	{
		base._Ready();

		open_cover.ButtonDown += OpenCover;
		playlist_cover.FileSelected += SetCover;

        add_songs.ButtonDown += OpenSongs;
		open_songs.FilesSelected += AddSongs;

    }

	public void Open()
	{
		Show();
	}

	public void OpenCover()
	{
		playlist_cover.Popup();
	}

	public void SetCover(string path)
	{

	}

	public void OpenSongs()
	{
		open_songs.Popup();
	}

	public void AddSongs(string[] paths)
	{
		foreach(string path in paths)
		{
			var disp = song_display.Instantiate() as PathDisplay;
            disp.path = path;
            disp.set_path();
            song_display_container.AddChild(disp);
			songs.Add(path);
        }
	}

	public override void _Process(double delta)
	{
		
	}
}
