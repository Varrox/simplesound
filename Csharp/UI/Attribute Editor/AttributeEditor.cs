using Godot;

public partial class AttributeEditor : EditorWindow
{
    [Export] public LineEdit Name, Artist, Sharelink;
    [Export] public Button CoverButton;
    [Export] public PathDisplay CoverLabel;
    [Export] public CheckBox ExplicitLyrics;
    [Export] public Button SubmitButton, CancelButton;

    public string song_name, artist, cover_path, share_link;
    public bool explicit_lyrics;
    bool cover_changed;

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
            bool changed = (Name.Text != song_name) || (Artist.Text != artist) || cover_changed || (explicit_lyrics != ExplicitLyrics.ButtonPressed) || (share_link != Sharelink.Text);
            SubmitButton.Visible = changed;
        }
    }

    public void Open(string currentSong, string currentArtist, string currentSharelink, bool explicitLyrics)
    {
        Name.Text = currentSong;
        song_name = currentSong;
        Artist.Text = currentArtist;
        artist = currentArtist;
        Sharelink.Text = currentSharelink;
        share_link = currentSharelink;

        ExplicitLyrics.ButtonPressed = explicitLyrics;
        this.explicit_lyrics = explicitLyrics;
        Show();
        Visible = true;
        CoverLabel.SetPath();
        cover_path = "";

        Globals.file_dialog.Reparent(this);
        Globals.file_dialog.FileSelected += SubmitCover;
        Globals.SetFileDialogCover();
    }

    public void Submit()
    {
        Visible = false;
        Hide();
        cover_changed = false;

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
        cover_changed = true;
    }

    public void SubmitCover(string path)
    {
        cover_path = path;
        CoverLabel.SetPath(path);
    }
}
