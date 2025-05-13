using Godot;
using System.Collections.Generic;

public partial class PlaylistCreator : EditorWindow
{
	[Export] TextEdit playlist_name;

	[Export] Button addCover;
	[Export] FileDialog addCoverDialog;
	[Export] PathDisplay coverDisplay;

	[Export] Button addSongs;
	[Export] FileDialog addSongsDialog;
	[Export] PackedScene songDisplay;
	[Export] Control songDisplayContainer;


    [Export] CheckBox album;
    [Export] TextEdit artist;

	[Export] CheckBox backgroundThemeEnabled;
	[Export] ColorPickerButton backgroundTheme;

    [Export] public Button SubmitButton, CancelButton;

    List<string> songs;
	public string name, cover_path;

	public override void _Ready()
	{
		base._Ready();

		songs = new List<string>();

		addCover.ButtonDown += OpenCover;
		addCoverDialog.FileSelected += SetCover;

        addSongs.ButtonDown += OpenSongs;
		addSongsDialog.FilesSelected += AddSongs;

		// Submit / Cancel

        SubmitButton.ButtonDown += Submit;
		CancelButton.ButtonDown += Cancel;
    }

	public void Open()
	{
		Show();
	}

	public void OpenCover()
	{
		addCoverDialog.Popup();
	}

	public void SetCover(string path)
	{
		cover_path = path;
		coverDisplay.set_path(cover_path);
	}

	public void OpenSongs()
	{
		addSongsDialog.Popup();
	}

	public void AddSongs(string[] paths)
	{
		foreach(string path in paths)
		{
			if(Tools.ValidAudioFile(path))
			{
				if(!songs.Contains(path))
				{
                    var disp = songDisplay.Instantiate() as PathDisplay;
                    disp.set_path(path);

                    songDisplayContainer.AddChild(disp);
                    songs.Add(path);
                }
                else
                {
                    Debug.ErrorLog($"{path} is already in this playlist.");
                }
            }
			else
			{
				Debug.ErrorLog($"{path} is not a valid audio file, it cannot be added to this playlist.");
			}
        }
	}

	public void RemoveSong(int index)
	{

	}

	public void Submit()
	{
        Visible = false;
        Hide();

        EmitSignal("OnClose");
    }
    public void Cancel()
    {
		cancelled = true;
		Submit();
    }
}
