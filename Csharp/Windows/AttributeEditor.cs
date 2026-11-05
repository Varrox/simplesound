using Godot;

public partial class AttributeEditor : EditorWindow {
    [Export] public StringInputField name_field, artist_field, share_link_field;
    [Export] public BooleanInputField explicit_lyrics_field;
    [Export] public FileInputField cover_input_field;

    [Export] public Button submit_button, cancel_button;

    public string track_path, track_name, artist, share_link;
    public bool explicit_lyrics;

    public override void _Ready()
    {
        base._Ready();

        submit_button.ButtonDown += Submit;
        cancel_button.ButtonDown += Cancel;
    }

    public override void _Process(double delta) {
        if (Visible) { 
            bool changed =  (name_field.value != track_name) || 
                            (artist_field.value != artist) || 
                            (share_link != share_link_field.value) ||
                            (explicit_lyrics != explicit_lyrics_field.value) || 
                            cover_input_field.value != "";
            
            submit_button.Visible = changed;
        }
    }

    public void Open(string current_track_name, string current_artist, string current_share_link, bool explicit_lyrics) {
        name_field.value = current_track_name;
        name_field.default_value = "";
        track_name = current_track_name;

        artist_field.value = current_artist;
        artist_field.default_value = "";
        artist = current_artist;

        share_link_field.value = current_share_link;
        share_link_field.default_value = "";
        share_link = current_share_link;

        explicit_lyrics_field.value = explicit_lyrics;
        explicit_lyrics_field.default_value = false;
        this.explicit_lyrics = explicit_lyrics;

        Show();
    }

    public void Open(TrackData track_data) {
        name_field.value = track_data.title;
        name_field.default_value = "";
        track_name = track_data.title;

        artist_field.value = track_data.artist;
        artist_field.default_value = "";
        artist = track_data.artist;

        share_link_field.value = track_data.share_link;
        share_link_field.default_value = "";
        share_link = track_data.share_link;

        explicit_lyrics_field.value = track_data.explicit_lyrics;
        explicit_lyrics_field.default_value = false;
        explicit_lyrics = track_data.explicit_lyrics;

        Show();
    }

    public void Submit() {
        Hide();

        EmitSignal(SignalName.OnClose);
        cancelled = false;
    }

    public void Cancel() {
        cancelled = true;
        Submit();
    }
}
