using Godot;
using Godot.Collections;
public partial class SongsMore : ContextMenuOpener
{
    [Export] public SongDisplay display;
    public static int song;

    public override void _Ready()
    {
        menu = Globals.song_menu;
        teleportMenu = true;

        OnOpen += () => song = display.song;

        base._Ready();
    }
}
