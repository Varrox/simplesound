using Godot;

public partial class AttributeEditorOpener : EditorWindowOpener
{
    public override void _Ready()
    {
        base._Ready();

        ButtonDown += EditAttributes;
        window.OnClose += SubmitMeta;
    }

    public void EditAttributes()
    {
        if (Globals.main.song != null) 
        {
            if (Globals.player.Interrupt())
            {
                (window as AttributeEditor).Open(Globals.player.SongName.Text, Globals.player.SongArtist.Text, Metadata.GetShareLink(Globals.main.song), Metadata.IsExplicit(Globals.main.song));
            }
        }
    }

    public void SubmitMeta()
    {
        AttributeEditor editor = window as AttributeEditor;

        if(!editor.cancelled)
        {
            if (Globals.main.playlist)
                Metadata.SetData(Globals.main.song, editor.Name.Text, editor.Artist.Text, editor.coverpath, editor.Sharelink.Text, editor.ExplicitLyrics.ButtonPressed);

            Globals.player.OnLoadSong();
            Globals.main.songsVisualizer.UpdateSong(Globals.main.currentSong, editor.Name.Text, editor.Artist.Text, Globals.player.TotalTime.Text, editor.ExplicitLyrics.ButtonPressed, Globals.player.SongCover.Texture);
        }

        Globals.player.interrupted = false;
    }
}