using Godot;

public partial class PlaylistCreatorOpener : EditorWindowOpener
{
    public override void _Ready()
    {
        base._Ready();

        ButtonDown += OpenCreator;
        window.OnClose += SubmitMeta;
    }

    public void OpenCreator()
    {
        if (Globals.player.interrupt())
        {
            (window as PlaylistCreator).Open();
        }
    }

    public void SubmitMeta()
    {
        PlaylistCreator creator = window as PlaylistCreator;

        if (!creator.cancelled)
        {
            
        }

        Globals.player.interrupted = false;
    }
}