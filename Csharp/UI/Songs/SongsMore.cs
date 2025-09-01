using Godot;
public partial class SongsMore : ContextMenu
{
    [Export] public SongDisplay display;

    public override void _Ready()
    {
        base._Ready();
        OnOpen += () => ShareMenu.song = display.song;

        menu = Globals.main.songs_visualizer.menu;

        sub_menus.Add(ShareMenu.instance);
    }
}
