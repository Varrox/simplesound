using Godot;
using Godot.Collections;

public partial class TracksContextMenuOpener : ContextMenuOpener
{
    [Export] public TrackDisplay display;
    public static int selected_track;

    public override void _Ready()
    {
        menu = Globals.track_menu;
        teleportMenu = true;

        OnOpen += () => selected_track = display.track_index;

        base._Ready();
    }
}
