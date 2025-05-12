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
        if (Globals.player.interrupt())
        {
            (window as AttributeEditor).open(Globals.player.SongName.Text, Globals.player.SongArtist.Text, Metadata.IsExplicit(Globals.main.song));
        }
    }

    public void SubmitMeta()
    {
        AttributeEditor editor = window as AttributeEditor;
        Globals.main.EditMeta(editor.songname, editor.artist, editor.coverpath, editor.explicitLyrics);
        Globals.player.onLoadSong();
        Globals.player.interrupted = false;
        Globals.main.songsVisualizer.UpdateSong(Globals.main.currentSong, editor.songname, editor.artist, Globals.player.TotalTime.Text, editor.explicitLyrics, Globals.player.SongCover.Texture);
    }
}