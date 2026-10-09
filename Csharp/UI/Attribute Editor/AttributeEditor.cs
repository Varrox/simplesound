using Godot;

public partial class AttributeEditor : EditorWindow
{
    [Export] public ThemeLineEdit name_field, artist_field, share_link_field;
    [Export] public CheckBox explicit_lyrics_field;

    [Export] public Button select_cover_button;
    [Export] public PathDisplay cover_label;

    [Export] public Button submit_button, cancel_button;

    public string track_name, artist, cover_path, share_link;
    public bool explicit_lyrics, cover_changed;

    public override void _Ready()
    {
        base._Ready();

        select_cover_button.ButtonDown += Cover;

        submit_button.ButtonDown += Submit;
        cancel_button.ButtonDown += Cancel;
    }

    public override void _Process(double delta)
    {
        if (Visible)
        { 
            bool changed = (name_field.Text != track_name) || (artist_field.Text != artist) || cover_changed || (explicit_lyrics != explicit_lyrics_field.ButtonPressed) || (share_link != share_link_field.Text);
            submit_button.Visible = changed;
        }
    }

    public void Open(string current_track_name, string current_artist, string current_share_link, bool explicit_lyrics)
    {
        name_field.Text = current_track_name;
        track_name = current_track_name;
        artist_field.Text = current_artist;
        artist = current_artist;
        share_link_field.Text = current_share_link;
        share_link = current_share_link;

        this.explicit_lyrics_field.ButtonPressed = explicit_lyrics;
        this.explicit_lyrics = explicit_lyrics;

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
