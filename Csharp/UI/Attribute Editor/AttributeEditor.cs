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
        SubmitButton.ButtonDown += submit;
        CoverButton.ButtonDown += cover;
        CancelButton.ButtonDown += Cancel;
        CoverFileDialog.FileSelected += submitCover;
    }

    public override void _Process(double delta)
    {
        if (Visible)
        { 
            bool changed = (Name.Text != songname) || (Artist.Text != artist) || coverChanged || (explicitLyrics != ExplicitLyrics.ButtonPressed);
            SubmitButton.Visible = changed;
        }
    }

    public void open(string currentSong, string currentArtist, bool explicitLyrics)
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

    public void submit()
    {
        songname = Name.Text;
        artist = Artist.Text;
        explicitLyrics = ExplicitLyrics.ButtonPressed;
        Visible = false;
        Hide();
        coverChanged = false;
        EmitSignal("OnClose");
    }

    public void Cancel()
    {
        Visible = false;
        Hide();
        coverChanged = false;
        EmitSignal("OnClose");
    }


    public void cover()
    {
        CoverFileDialog.Popup();
        coverChanged = true;
    }

    public void submitCover(string path)
    {
        coverpath = path;
        CoverLabel.Text = path;
    }
}
