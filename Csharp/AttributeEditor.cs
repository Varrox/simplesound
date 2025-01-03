using Godot;
using System;

public partial class AttributeEditor : Node
{
    [Export] public TextEdit Name, Artist;
    [Export] public Button CoverButton;
    [Export] public FileDialog CoverFileDialog;
    [Export] public Label CoverLabel;
    [Export] public Button SubmitButton;
    [Export] public Window AttributeWindow;

    [Signal] public delegate void onSubmitdataEventHandler();

    public string songname, artist, coverpath;
    bool coverChanged;

    public override void _Ready()
    {
        SubmitButton.ButtonDown += submit;
        CoverButton.ButtonDown += cover;
    }

    public override void _Process(double delta)
    {
        if (AttributeWindow.Visible)
        { 
            bool changed = (Name.Text != songname) || (Artist.Text != artist) || coverChanged;
            SubmitButton.Text = changed ? "Apply Changes" : "Cancel";
        }
    }

    public void open(string currentSong, string currentArtist)
    {
        Name.Text = currentSong;
        songname = currentSong;
        Artist.Text = currentArtist;
        artist = currentArtist;
        AttributeWindow.Show();
        AttributeWindow.Visible = true;
        CoverLabel.Text = "";
        coverpath = "";
    }

    public void submit()
    {
        songname = Name.Text;
        artist = Artist.Text;
        AttributeWindow.Visible = false;
        AttributeWindow.Hide();
        coverChanged = false;
        EmitSignal("onSubmitdata");
    }


    public void cover()
    {
        CoverFileDialog.Popup();
        coverpath = CoverFileDialog.CurrentFile;
        CoverLabel.Text = coverpath;
        coverChanged = true;
    }
}
