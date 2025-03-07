using Godot;

public partial class AttributeEditor : Node
{
    [Export] public TextEdit Name, Artist;
    [Export] public Button CoverButton;
    [Export] public FileDialog CoverFileDialog;
    [Export] public Label CoverLabel;
    [Export] public CheckBox ExplicitLyrics;
    [Export] public Button SubmitButton, CancelButton;
    [Export] public Window AttributeWindow;

    [Signal] public delegate void onSubmitdataEventHandler();

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
        if (AttributeWindow.Visible)
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
        AttributeWindow.Show();
        AttributeWindow.Visible = true;
        CoverLabel.Text = "";
        coverpath = "";
    }

    public void submit()
    {
        songname = Name.Text;
        artist = Artist.Text;
        explicitLyrics = ExplicitLyrics.ButtonPressed;
        AttributeWindow.Visible = false;
        AttributeWindow.Hide();
        coverChanged = false;
        EmitSignal("onSubmitdata");
    }

    public void Cancel()
    {
        AttributeWindow.Visible = false;
        AttributeWindow.Hide();
        coverChanged = false;
        EmitSignal("onSubmitdata");
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
