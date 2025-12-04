using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;

public partial class ContextMenu : Button
{
    [Export] public Control menu;
    [Export] public bool teleportMenu;
    [Export] public Vector2 teleportMenuOffset;
    [Export] public Array<ContextMenu> sub_menus;
    public bool menu_open;

    public Action OnClose;
    public Action OnOpen;

    public override void _Ready()
    {
        ButtonDown += ToggleMenu;

        if (sub_menus == null)
            sub_menus = new Array<ContextMenu> ();

        if(menu != null)
        {
            menu_open = false;
            menu.Visible = false;
        }
    }

    public override void _Process(double delta)
    {
        if (menu_open && !Globals.player.interrupted)
        {
            if (!DisplayServer.WindowIsFocused())
            {
                CloseMenu();
            }

            if(Input.IsMouseButtonPressed(MouseButton.Left) || Input.IsMouseButtonPressed(MouseButton.Right) || Input.IsMouseButtonPressed(MouseButton.Middle))
            {
                bool self = IsMouseInRect(menu) || IsHovered();

                bool sub_menus_intersected = CheckSubMenusIntersection();

                if (!self && !sub_menus_intersected)
                {
                    CloseMenu();
                }
            }
        }
    }

    public static bool IsMouseInRect(Control control)
    {
        return control.GetGlobalRect().HasPoint(control.GetGlobalMousePosition());
    }

    public void ToggleMenu()
    {
        menu_open = !menu_open;
        if (menu_open && teleportMenu) TeleportMenu();
        menu.Visible = menu_open;
        (menu_open ? OnOpen : OnClose)?.Invoke();

        if (!menu_open)
        {
            for (int i = 0; i < sub_menus.Count; i++)
            {
                sub_menus[i].CloseMenu();
            }
        }
    }

    public void CloseMenu()
    {
        menu_open = false;
        menu.Visible = false;
        OnClose?.Invoke();

        for (int i = 0; i < sub_menus.Count; i++)
        {
            sub_menus[i].CloseMenu();
        }
    }

    public void OpenMenu()
    {
        menu_open = true;
        if (menu_open && teleportMenu) TeleportMenu();
        menu.Visible = true;
        OnOpen?.Invoke();
    }

    public bool CheckSubMenusIntersection()
    {
        foreach (ContextMenu submenu in sub_menus)
        {
            if (submenu.menu_open)
            {
                if (submenu.CheckSubMenusIntersection() || (IsMouseInRect(submenu.menu) || submenu.IsHovered()))
                {
                    return true;
                }
            }
        }

        return false;
    }

    public void TeleportMenu()
    {
        menu.GlobalPosition = GlobalPosition + teleportMenuOffset - menu.PivotOffset;
    }

    public override void _ExitTree()
    {
        CloseMenu();
    }
}
