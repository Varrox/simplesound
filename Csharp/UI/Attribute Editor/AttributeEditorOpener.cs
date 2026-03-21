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
        if (Globals.main.song != null) 
        {
            if (!Globals.player.Interrupt())
            {
                return;
            }

            (window as AttributeEditor).Open(Globals.player.song_name.Text, Globals.player.song_artist.Text, Metadata.GetShareLink(Globals.main.song), Metadata.IsExplicit(Globals.main.song));
            window.OnClose += SubmitMeta;
        }
    }

    public void SubmitMeta()
    {
        AttributeEditor editor = window as AttributeEditor;

        if(!editor.cancelled)
        {
            if (Globals.main.playlist != null)
                Metadata.SetData(Globals.main.song, editor.name.Text, editor.artist.Text, editor.cover_path, editor.share_link.Text, editor.explicit_lyrics.ButtonPressed);

            Globals.player.OnLoadSong();

            SongData song_data = new SongData
            {
                title = editor.name.Text,
                artist = editor.artist.Text,
                time = Globals.player.total_time.Text,
                explicit_lyrics = editor.explicit_lyrics.ButtonPressed,
                corrupt = Metadata.IsFileCorrupt(Globals.main.song)
            };

            Globals.main.songs_visualizer.UpdateSong(Globals.main.song_index, song_data, ConvertToGodot.GetCover(Globals.main.song));
        }

        Globals.player.interrupted = false;
        window.OnClose -= SubmitMeta;
    }
}