using Godot;
using System;

public partial class PlaylistsMore : ContextMenuOpener
{
    public override void _Ready()
    {
        Hide();
        
        MouseEntered += Show;
        OnClose += Hide;

        menu = Globals.playlist_menu;
        teleportMenu = true;
        
        base._Ready();
    }

}
