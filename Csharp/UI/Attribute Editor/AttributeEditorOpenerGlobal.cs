using Godot;
using System;

public partial class AttributeEditorOpenerGlobal : EditorWindowOpener
{
    string file;
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

            file = Globals.main.playlists[Globals.main.looked_at_playlist].songs[TracksContextMenuOpener.selected_track];

            (window as AttributeEditor).Open(Metadata.GetName(file), Metadata.GetArtist(file), Metadata.GetShareLink(file), Metadata.IsExplicit(file));
            window.OnClose += SubmitMeta;
        }
    }

    public void SubmitMeta()
    {
        AttributeEditor editor = window as AttributeEditor;

        if (!editor.cancelled)
        {
            if (Globals.main.playlist != null)
                Metadata.SetData(file, editor.name_field.Text, editor.artist_field.Text, editor.cover_path, editor.share_link_field.Text, editor.explicit_lyrics_field.ButtonPressed);

            Globals.player.OnLoadTrack();

            TrackData track_data = new TrackData
            {
                title = editor.name_field.Text,
                artist = editor.artist_field.Text,
                time = Tools.SecondsToTimestamp(Metadata.GetTotalTime(file)),
                explicit_lyrics = editor.explicit_lyrics_field.ButtonPressed,
                corrupt = Metadata.IsFileCorrupt(file)
            };

            Globals.main.tracks_visualizer.UpdateTrack(TracksContextMenuOpener.selected_track, track_data, ConvertToGodot.GetMediaCover(file));
        }

        Globals.player.interrupted = false;
        window.OnClose -= SubmitMeta;
    }
}
