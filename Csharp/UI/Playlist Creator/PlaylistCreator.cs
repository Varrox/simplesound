using Godot;
using System.Collections.Generic;

public partial class PlaylistCreator : EditorWindow
{
	[Export] public ThemeLineEdit playlist_name_field;

	[Export] public Button add_track_button;
	[Export] public Panel track_panel;
	[Export] public Control track_path_display_container;

	[Export] public Button add_cover_button;
	[Export] public PathDisplay cover_path_display;

	[Export] public CheckBox album_field;
	[Export] public ThemeLineEdit artist_field;

	[Export] public CheckBox background_color_enabled_field;
	[Export] public ThemeColorPickerButton background_color_field;

	[Export] public CheckBox cloud_sync_enabled_field;

	[Export] public Button submit_button, cancel_button;

	public List<string> tracks = new List<string>();
	public string cover_path;

	bool cleared = false;

	public override void _Ready()
	{
		base._Ready();

		add_cover_button.ButtonUp += OpenCover;
		cover_path_display.delete.ButtonUp += ClearCover;

		add_track_button.ButtonUp += OpenTracks;

		// Submit / Cancel

		submit_button.ButtonUp += Submit;
		cancel_button.ButtonUp += Cancel;
	}

	public void Clear()
	{
        playlist_name_field.Text = "";
        cover_path_display.SetPath();

        foreach (Node child in track_path_display_container.GetChildren())
        {
            child.QueueFree();
        }

        tracks.Clear();

        album_field.ButtonPressed = false;
        artist_field.Text = "";
        background_color_enabled_field.ButtonPressed = false;
        background_color_field.Color = Colors.White;

		cleared = true;
    }

	public void Open()
	{
		if (!cleared)
			Clear();

        Globals.file_dialog.Reparent(this);
		FilesDropped += DropTracks;

        Show();
	}

    public override void _Process(double delta)
    {
		background_color_field.Disabled = !background_color_enabled_field.ButtonPressed;
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
		cover_path_display.SetPath(cover_path);
        CancelSetCover();
    }
	
	public void ClearCover()
	{
		cover_path = "";
        cover_path_display.SetPath(cover_path);
    }

	public void OpenTracks()
	{
		Globals.SetFileDialogTracks();
		Globals.file_dialog.Popup();

        Globals.file_dialog.FilesSelected += AddTracks;
        Globals.file_dialog.Canceled += CancelAddTracks;
    }

	void CancelAddTracks() { Globals.file_dialog.FilesSelected -= AddTracks; Globals.file_dialog.Canceled -= CancelAddTracks; }
    void CancelSetCover() { Globals.file_dialog.FileSelected -= SetCover; Globals.file_dialog.Canceled -= CancelSetCover; }

	public void DropTracks(string[] files)
	{
		if(track_panel.GetGlobalRect().HasPoint(GetMousePosition()))
		{
			AddTracks(files);
		}	
	}

    public void AddTracks(string[] paths)
	{
		foreach(string path in paths)
		{
			if(Tools.ValidAudioFile(path))
			{
				if(!tracks.Contains(path))
				{
					var disp = Globals.path_display.Instantiate() as PathDisplay;
					disp.SetPath(path);
					
					track_path_display_container.AddChild(disp);
					tracks.Add(path);

					disp.delete.ButtonUp += () => tracks.Remove(path);
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
        CancelAddTracks();
    }

	public void Submit()
	{
		Visible = false;
		Hide();

        Globals.file_dialog.Reparent(Globals.self);
		FilesDropped -= DropTracks;

        OnClose?.Invoke();
    }
	public void Cancel()
	{
		cancelled = true;
		Submit();
	}
}
