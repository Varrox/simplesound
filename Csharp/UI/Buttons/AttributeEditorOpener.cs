using Godot;

public partial class AttributeEditorOpener : Button {
    public override void _Ready() {
        ButtonUp += EditAttributes;
    }

    AttributeEditor attribute_editor;

    public void EditAttributes() {
        if (Main.GetCurrentTrack() != null) {
            Main.instance.Pause();

            attribute_editor = (AttributeEditor)ApplicationManager.GetSceneResource("metadata_editor_window").Instantiate();
            GetTree().Root.AddChild(attribute_editor);

            attribute_editor.Open(Player.instance.track_name_label.Text, Player.instance.track_artist_label.Text, Metadata.GetShareLink(Main.GetCurrentTrack()), Metadata.IsExplicit(Main.GetCurrentTrack()));
            attribute_editor.OnClose += SubmitMeta;
        }
    }

    public void SubmitMeta() {
        if(!attribute_editor.cancelled) {
            if (Main.GetCurrentPlaylist() != null)
                Metadata.SetData(Main.GetCurrentTrack(), attribute_editor.name_field.value, attribute_editor.artist_field.value, attribute_editor.cover_input_field.value, attribute_editor.share_link_field.value, attribute_editor.explicit_lyrics_field.value);

            Player.instance.UpdateDisplayedInfo();

            TrackData track_data = new TrackData {
                title = attribute_editor.name_field.value,
                artist = attribute_editor.artist_field.value,
                share_link = attribute_editor.share_link_field.value,
                explicit_lyrics = attribute_editor.explicit_lyrics_field.value,
                time = Player.instance.total_time_label.Text,
                corrupt = Metadata.IsFileCorrupt(Main.GetCurrentTrack()),
            };

            Texture2D default_cover_icon = ApplicationManager.GetThemeIcon("default_cover_icon");
            Main.instance.tracks_visualizer.UpdateTrack(Main.track_index, track_data, Tools.GetMediaCover(Main.GetCurrentTrack()) ?? default_cover_icon);
        }
        
        attribute_editor.OnClose -= SubmitMeta;
        
        attribute_editor.QueueFree();

        attribute_editor = null;
    }
}