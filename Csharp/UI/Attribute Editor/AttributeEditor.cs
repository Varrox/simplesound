using Godot;

public partial class AttributeEditor : EditorWindow
{
    [Export] public TextEdit Name, Artist, Sharelink;
    [Export] public Button CoverButton;
    [Export] public PathDisplay CoverLabel;
    [Export] public CheckBox ExplicitLyrics;
    [Export] public Button SubmitButton, CancelButton;

    public string songname, artist, coverpath, sharelink;
    public bool explicitLyrics;
    bool coverChanged;

    public override void _Ready()
    {
        SubmitButton.ButtonDown += Submit;
        CoverButton.ButtonDown += Cover;
        CancelButton.ButtonDown += Cancel;
    }

    public override void _Process(double delta)
    {
        if (Visible)
        { 
            bool changed = (Name.Text != songname) || (Artist.Text != artist) || coverChanged || (explicitLyrics != ExplicitLyrics.ButtonPressed) || (sharelink != Sharelink.Text);
            SubmitButton.Visible = changed;
        }
    }

    public void Open(string currentSong, string currentArtist, string currentSharelink, bool explicitLyrics)
    {
        Name.Text = currentSong;
        songname = currentSong;
        Artist.Text = currentArtist;
        artist = currentArtist;
        Sharelink.Text = currentSharelink;
        sharelink = currentSharelink;

        ExplicitLyrics.ButtonPressed = explicitLyrics;
        this.explicitLyrics = explicitLyrics;
        Show();
        Visible = true;
        CoverLabel.set_path();
        coverpath = "";

        Globals.file_dialog.Reparent(this);
        Globals.file_dialog.FileSelected += SubmitCover;
        Globals.SetFileDialogCover();
    }

    public void Submit()
    {
        Visible = false;
        Hide();
        coverChanged = false;

        EmitSignal("OnClose");
        cancelled = false;

        Globals.file_dialog.Reparent(Globals.self);
        Globals.file_dialog.FileSelected -= SubmitCover;
        Globals.ResetFileDialogParameters();
    }

    public void Cancel()
    {
        cancelled = true;
        Submit();
    }

    public void Cover()
    {
        Globals.file_dialog.Popup();
        coverChanged = true;
    }

    public void SubmitCover(string path)
    {
        coverpath = path;
        CoverLabel.set_path(path);
    }
}
