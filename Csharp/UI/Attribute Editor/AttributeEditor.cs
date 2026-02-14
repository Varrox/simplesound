using Godot;

public partial class AttributeEditor : EditorWindow
{
    [Export] public LineEdit name, artist, share_link;
    [Export] public Button cover_button;
    [Export] public PathDisplay cover_label;
    [Export] public CheckBox explicit_lyrics;
    [Export] public Button submit_button, cancel_button;

    public string song_name, _artist, cover_path, _share_link;
    public bool _explicit_lyrics, cover_changed;

    public override void _Ready()
    {
        base._Ready();

        submit_button.ButtonDown += Submit;
        cover_button.ButtonDown += Cover;
        cancel_button.ButtonDown += Cancel;
    }

    public override void _Process(double delta)
    {
        if (Visible)
        { 
            bool changed = (name.Text != song_name) || (artist.Text != _artist) || cover_changed || (_explicit_lyrics != explicit_lyrics.ButtonPressed) || (_share_link != share_link.Text);
            submit_button.Visible = changed;
        }
    }

    public void Open(string currentSong, string current_artist, string current_share_link, bool explicit_lyrics)
    {
        name.Text = currentSong;
        song_name = currentSong;
        artist.Text = current_artist;
        _artist = current_artist;
        share_link.Text = current_share_link;
        _share_link = current_share_link;

        this.explicit_lyrics.ButtonPressed = explicit_lyrics;
        this._explicit_lyrics = explicit_lyrics;

        cover_label.SetPath();
        cover_path = "";

        Show();
    }

    public void Submit()
    {
        Hide();

        cover_changed = false;

        OnClose?.Invoke();
        cancelled = false;
    }

    public void Cancel()
    {
        cancelled = true;
        Submit();
    }

    public void Cover()
    {
        Globals.file_dialog.Reparent(this);
        Globals.file_dialog.FileSelected += SubmitCover;

        Globals.SetFileDialogCover();
        Globals.file_dialog.Popup();

        cover_changed = true;
    }

    public void SubmitCover(string path)
    {
        cover_path = path;
        cover_label.SetPath(path);

        Globals.file_dialog.Reparent(Globals.self);
        Globals.file_dialog.FileSelected -= SubmitCover;

        Globals.ResetFileDialogParameters();
    }
}
