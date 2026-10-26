using Godot;

public partial class AttributeEditorOpener : EditorWindowOpener {
    public override void _Ready() {
        ButtonUp += EditAttributes;
        window = Globals.attribute_editor;
    }

    public void EditAttributes() {
        if (Main.GetCurrentTrack() != null) {
            Main.instance.Pause();

            (window as AttributeEditor).Open(Player.instance.track_name_label.Text, Player.instance.track_artist_label.Text, Metadata.GetShareLink(Main.GetCurrentTrack()), Metadata.IsExplicit(Main.GetCurrentTrack()));
            window.OnClose += SubmitMeta;
        }
    }

    public void SubmitMeta() {
        AttributeEditor editor = window as AttributeEditor;

        if(!editor.cancelled) {
            if (Main.GetCurrentPlaylist() != null)
                Metadata.SetData(Main.GetCurrentTrack(), editor.name_field.Text, editor.artist_field.Text, editor.cover_path, editor.share_link_field.Text, editor.explicit_lyrics_field.ButtonPressed);

            Player.instance.UpdateDisplayedInfo();

            TrackData track_data = new TrackData {
                title = editor.name_field.Text,
                artist = editor.artist_field.Text,
                share_link = editor.share_link_field.Text,
                explicit_lyrics = editor.explicit_lyrics_field.ButtonPressed,
                time = Player.instance.total_time_label.Text,
                corrupt = Metadata.IsFileCorrupt(Main.GetCurrentTrack()),
            };

            Texture2D default_cover_icon = ApplicationManager.GetThemeIcon("default_cover_icon");
            Main.instance.tracks_visualizer.UpdateTrack(Main.track_index, track_data, Tools.GetMediaCover(Main.GetCurrentTrack()) ?? default_cover_icon);
        }
        
        window.OnClose -= SubmitMeta;
    }
}