using Godot;
using System.Collections.Generic;

public partial class PlaylistCreator : EditorWindow
{
	[Export] public TextEdit playlist_name;

	[Export] Button addCover;
	[Export] FileDialog addCoverDialog;
	[Export] PathDisplay coverDisplay;

	[Export] Button addSongs;
	[Export] FileDialog addSongsDialog;
	[Export] PackedScene songDisplay;
	[Export] Control songDisplayContainer;


	[Export] public CheckBox album;
	[Export] public TextEdit artist;

	[Export] public CheckBox backgroundThemeEnabled;
	[Export] public ColorPickerButton backgroundTheme;

	[Export] public Button SubmitButton, CancelButton;

	public List<string> songs;
	public string cover_path;

	public override void _Ready()
	{
		base._Ready();

		songs = new List<string>();

		addCover.ButtonDown += OpenCover;
		addCoverDialog.FileSelected += SetCover;
		coverDisplay.delete.ButtonDown += ClearCover;

		addSongs.ButtonDown += OpenSongs;
		addSongsDialog.FilesSelected += AddSongs;

		// Submit / Cancel

		SubmitButton.ButtonDown += Submit;
		CancelButton.ButtonDown += Cancel;
	}

	public void Open()
	{
		playlist_name.Text = "";
        coverDisplay.set_path();

        foreach (Node child in songDisplayContainer.GetChildren())
        {
            child.QueueFree();
        }

        album.ButtonPressed = false;
		artist.Text = "";
		backgroundThemeEnabled.ButtonPressed = false;
		backgroundTheme.Color = new Color(1, 1, 1);

		Show();
	}

    public override void _Process(double delta)
    {
		backgroundTheme.Disabled = !backgroundThemeEnabled.ButtonPressed;
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
	
	public void ClearCover()
	{
		cover_path = "";
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

					disp.delete.ButtonDown += () => songs.Remove(path);
					disp.delete.ButtonDown += () => disp.QueueFree();
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
