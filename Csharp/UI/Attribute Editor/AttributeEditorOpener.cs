using Godot;

public partial class AttributeEditorOpener : EditorWindowOpener
{
    public override void _Ready()
    {
        ButtonUp += EditAttributes;
        window.OnClose += SubmitMeta;
    }

    public void EditAttributes()
    {
        if (Globals.main.song != null) 
        {
            if (!Globals.player.Interrupt())
            {
                return;
            }

            (window as AttributeEditor).Open(Globals.player.SongName.Text, Globals.player.SongArtist.Text, Metadata.GetShareLink(Globals.main.song), Metadata.IsExplicit(Globals.main.song));
        }
    }

    public void SubmitMeta()
    {
        AttributeEditor editor = window as AttributeEditor;

        if(!editor.cancelled)
        {
            if (Globals.main.playlist != null)
                Metadata.SetData(Globals.main.song, editor.Name.Text, editor.Artist.Text, editor.cover_path, editor.Sharelink.Text, editor.ExplicitLyrics.ButtonPressed);

            Globals.player.OnLoadSong();
            Globals.main.songs_visualizer.UpdateSong(Globals.main.song_index, editor.Name.Text, editor.Artist.Text, Globals.player.TotalTime.Text, editor.ExplicitLyrics.ButtonPressed, Globals.player.SongCover.Texture);
        }

        Globals.player.interrupted = false;
    }
}