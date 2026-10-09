using Godot;

public partial class AttributeEditorOpener : EditorWindowOpener
{
    public override void _Ready()
    {
        ButtonUp += EditAttributes;
        window = Globals.attribute_editor;
    }

    public void EditAttributes()
    {
        if (Globals.main.track != null) 
        {
            if (!Globals.player.Interrupt())
            {
                return;
            }

            (window as AttributeEditor).Open(Globals.player.track_name_label.Text, Globals.player.track_artist_label.Text, Metadata.GetShareLink(Globals.main.track), Metadata.IsExplicit(Globals.main.track));
            window.OnClose += SubmitMeta;
        }
    }

    public void SubmitMeta()
    {
        AttributeEditor editor = window as AttributeEditor;

        if(!editor.cancelled)
        {
            if (Globals.main.playlist != null)
                Metadata.SetData(Globals.main.track, editor.name_field.Text, editor.artist_field.Text, editor.cover_path, editor.share_link_field.Text, editor.explicit_lyrics_field.ButtonPressed);

            Globals.player.OnLoadTrack();

            TrackData track_data = new TrackData
            {
                title = editor.name_field.Text,
                artist = editor.artist_field.Text,
                time = Globals.player.total_time_label.Text,
                explicit_lyrics = editor.explicit_lyrics_field.ButtonPressed,
                corrupt = Metadata.IsFileCorrupt(Globals.main.track)
            };

            Globals.main.tracks_visualizer.UpdateTrack(Globals.main.track_index, track_data, ConvertToGodot.GetMediaCover(Globals.main.track));
        }

        Globals.player.interrupted = false;
        window.OnClose -= SubmitMeta;
    }
}