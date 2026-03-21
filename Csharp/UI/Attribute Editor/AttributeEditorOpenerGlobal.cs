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
        if (Globals.main.song != null)
        {
            if (!Globals.player.Interrupt())
            {
                return;
            }

            file = Globals.main.playlists[Globals.main.looked_at_playlist].songs[SongsMore.song];

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
                Metadata.SetData(file, editor.name.Text, editor.artist.Text, editor.cover_path, editor.share_link.Text, editor.explicit_lyrics.ButtonPressed);

            Globals.player.OnLoadSong();

            SongData song_data = new SongData
            {
                title = editor.name.Text,
                artist = editor.artist.Text,
                time = Tools.SecondsToTimestamp(Metadata.GetTotalTime(file)),
                explicit_lyrics = editor.explicit_lyrics.ButtonPressed,
                corrupt = Metadata.IsFileCorrupt(file)
            };

            Globals.main.songs_visualizer.UpdateSong(SongsMore.song, song_data, ConvertToGodot.GetCover(file));
        }

        Globals.player.interrupted = false;
        window.OnClose -= SubmitMeta;
    }
}
