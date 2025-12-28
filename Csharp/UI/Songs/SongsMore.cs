using Godot;
using Godot.Collections;
public partial class SongsMore : ContextMenu
{
    [Export] public SongDisplay display;
    public static int song;

    public override void _Ready()
    {
        base._Ready();

        menu = Globals.song_menu;
        teleportMenu = true;
        
        sub_menus.Add(ShareMenu.instance);

        OnOpen += () => song = display.song;
    }
}
