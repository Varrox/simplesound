using Godot;
using System.Collections.Generic;

public partial class PlaylistCreator : EditorWindow
{
	[Export] public LineEdit playlist_name;

	[Export] public Button addCover, addSongs;
	[Export] public PathDisplay coverDisplay;

	[Export] public PackedScene songDisplay;
	[Export] public Control songDisplayContainer;
	[Export] public Panel panel;

	[Export] public CheckBox cloudSync, album, backgroundThemeEnabled;
	[Export] public LineEdit artist;
	[Export] public ColorPickerButton backgroundTheme;

	[Export] public Button SubmitButton, CancelButton;

	public List<string> songs = new List<string>();
	public string cover_path;

	bool cleared = false;

	public override void _Ready()
	{
		base._Ready();

		addCover.ButtonUp += OpenCover;
		coverDisplay.delete.ButtonUp += ClearCover;

		addSongs.ButtonUp += OpenSongs;

		// Submit / Cancel

		SubmitButton.ButtonUp += Submit;
		CancelButton.ButtonUp += Cancel;
	}

	public void Clear()
	{
        playlist_name.Text = "";
        coverDisplay.SetPath();

        foreach (Node child in songDisplayContainer.GetChildren())
        {
            child.QueueFree();
        }

        songs.Clear();

        album.ButtonPressed = false;
        artist.Text = "";
        backgroundThemeEnabled.ButtonPressed = false;
        backgroundTheme.Color = new Color(1, 1, 1);

		cleared = true;
    }

	public void Open()
	{
		if (!cleared)
			Clear();

        Globals.file_dialog.Reparent(this);
		FilesDropped += DropSongs;

        Show();
	}

    public override void _Process(double delta)
    {
		backgroundTheme.Disabled = !backgroundThemeEnabled.ButtonPressed;
    }

    public void OpenCover()
	{
        Globals.SetFileDialogCover();
        Globals.file_dialog.Popup();

		Globals.file_dialog.FileSelected += SetCover;
        Globals.file_dialog.Canceled += CancelSetCover;
    }

	public void SetCover(string path)
	{
		cover_path = path;
		coverDisplay.SetPath(cover_path);
        CancelSetCover();
    }
	
	public void ClearCover()
	{
		cover_path = "";
        coverDisplay.SetPath(cover_path);
    }

	public void OpenSongs()
	{
		Globals.SetFileDialogSongs();
		Globals.file_dialog.Popup();

        Globals.file_dialog.FilesSelected += AddSongs;
        Globals.file_dialog.Canceled += CancelAddSongs;
    }

	void CancelAddSongs() { Globals.file_dialog.FilesSelected -= AddSongs; Globals.file_dialog.Canceled -= CancelAddSongs; }
    void CancelSetCover() { Globals.file_dialog.FileSelected -= SetCover; Globals.file_dialog.Canceled -= CancelSetCover; }

	public void DropSongs(string[] files)
	{
		if(panel.GetGlobalRect().HasPoint(GetMousePosition()))
		{
			AddSongs(files);
		}	
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
					disp.SetPath(path);
					
					songDisplayContainer.AddChild(disp);
					songs.Add(path);

					disp.delete.ButtonUp += () => songs.Remove(path);
					disp.delete.ButtonUp += () => disp.QueueFree();
				}
				else
				{
					GD.PushError($"{path} is already in this playlist.");
				}
			}
			else
			{
                GD.PushError($"{path} is not a valid audio file, it cannot be added to this playlist.");
			}
		}
        CancelAddSongs();
    }

	public void Submit()
	{
		Visible = false;
		Hide();

        Globals.file_dialog.Reparent(Globals.self);
		FilesDropped -= DropSongs;

        OnClose?.Invoke();
    }
	public void Cancel()
	{
		cancelled = true;
		Submit();
	}
}
