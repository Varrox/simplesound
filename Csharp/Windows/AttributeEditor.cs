using Godot;

public partial class AttributeEditor : EditorWindow {
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

    public override void _Process(double delta) {
        if (Visible) { 
            bool changed = (name_field.Text != track_name) || (artist_field.Text != artist) || cover_changed || (explicit_lyrics != explicit_lyrics_field.ButtonPressed) || (share_link != share_link_field.Text);
            submit_button.Visible = changed;
        }
    }

    public void Open(string current_track_name, string current_artist, string current_share_link, bool explicit_lyrics) {
        name_field.Text = current_track_name;
        track_name = current_track_name;
        artist_field.Text = current_artist;
        artist = current_artist;
        share_link_field.Text = current_share_link;
        share_link = current_share_link;

        this.explicit_lyrics_field.ButtonPressed = explicit_lyrics;
        this.explicit_lyrics = explicit_lyrics;

        cover_path = "";

        Show();
    }

    public void Open(TrackData track_data) {
        name_field.Text = track_data.title;
        track_name = track_data.title;
        artist_field.Text = track_data.artist;
        artist = track_data.artist;
        share_link_field.Text = track_data.share_link;
        share_link = track_data.share_link;

        this.explicit_lyrics_field.ButtonPressed = track_data.explicit_lyrics;
        this.explicit_lyrics = track_data.explicit_lyrics;

        cover_path = "";

        Show();
    }

    public void Submit() {
        Hide();

        cover_changed = false;

        EmitSignal(SignalName.OnClose);
        cancelled = false;
    }

    public void Cancel() {
        cancelled = true;
        Submit();
    }

    public void Cover() {
        ThemeFileDialog file_dialog = new ThemeFileDialog();
        
        AddChild(file_dialog);

        file_dialog.SetCover();
        file_dialog.Popup();
        

        void SubmitCover(string path) {
            cover_path = path;
            cover_label.path = path;

            Finish();
        }

        void Finish() {
            file_dialog.FileSelected -= SubmitCover;
            file_dialog.Canceled -= Finish;

            file_dialog.QueueFree();
        }

        file_dialog.FileSelected += SubmitCover;
        file_dialog.Canceled += Finish;

        cover_changed = true;
    }
}
