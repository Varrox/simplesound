using Godot;

public partial class AttributeEditor : EditorWindow
{
    [Export] public TextEdit Name, Artist;
    [Export] public Button CoverButton;
    [Export] public FileDialog CoverFileDialog;
    [Export] public Label CoverLabel;
    [Export] public CheckBox ExplicitLyrics;
    [Export] public Button SubmitButton, CancelButton;

    public string songname, artist, coverpath;
    public bool explicitLyrics;
    bool coverChanged;

    public override void _Ready()
    {
        SubmitButton.ButtonDown += Submit;
        CoverButton.ButtonDown += Cover;
        CancelButton.ButtonDown += Cancel;
        CoverFileDialog.FileSelected += SubmitCover;
    }

    public override void _Process(double delta)
    {
        if (Visible)
        { 
            bool changed = (Name.Text != songname) || (Artist.Text != artist) || coverChanged || (explicitLyrics != ExplicitLyrics.ButtonPressed);
            SubmitButton.Visible = changed;
        }
    }

    public void Open(string currentSong, string currentArtist, bool explicitLyrics)
    {
        Name.Text = currentSong;
        songname = currentSong;
        Artist.Text = currentArtist;
        artist = currentArtist;
        ExplicitLyrics.ButtonPressed = explicitLyrics;
        this.explicitLyrics = explicitLyrics;
        Show();
        Visible = true;
        CoverLabel.Text = "";
        coverpath = "";
    }

    public void Submit()
    {
        Visible = false;
        Hide();
        coverChanged = false;

        EmitSignal("OnClose");
        cancelled = false;
    }

    public void Cancel()
    {
        cancelled = true;
        Submit();
    }

    public void Cover()
    {
        CoverFileDialog.Popup();
        coverChanged = true;
    }

    public void SubmitCover(string path)
    {
        coverpath = path;
        CoverLabel.Text = path;
    }
}
