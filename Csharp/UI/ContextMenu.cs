using Godot;

public partial class ContextMenu : Button
{
    [Export] public Control menu;
    [Export] public bool teleportMenu;
    [Export] public Vector2 teleportMenuOffset;
    public bool menu_open;

    [Signal] public delegate void OnCloseEventHandler();
    public override void _Ready()
    {
        ButtonDown += ToggleMenu;

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
                if (IsMouseInRect(menu) && !IsHovered())
                {
                    CloseMenu();
                }
            }
        }
    }

    public static bool IsMouseInRect(Control control)
    {
        Vector2I mouse_position = DisplayServer.MouseGetPosition() - DisplayServer.WindowGetPosition();
        Rect2 rect = control.GetGlobalRect();
        Vector2 rect_position = rect.GetCenter();
        bool x = mouse_position.X > rect_position.X + Mathf.Abs(rect.Size.X / 2) || mouse_position.X < rect_position.X - Mathf.Abs(rect.Size.X / 2);
        bool y = mouse_position.Y > rect_position.Y + Mathf.Abs(rect.Size.Y / 2) || mouse_position.Y < rect_position.Y - Mathf.Abs(rect.Size.Y / 2);
        return x || y;
    }

    public void ToggleMenu()
    {
        menu_open = !menu_open;
        if (menu_open && teleportMenu) TeleportMenu();
        menu.Visible = menu_open;
    }

    public void CloseMenu()
    {
        menu_open = false;
        menu.Visible = false;
        EmitSignal("OnClose");
    }

    public void OpenMenu()
    {
        menu_open = true;
        if (menu_open && teleportMenu) TeleportMenu();
        menu.Visible = true;
    }

    public void TeleportMenu()
    {
        menu.GlobalPosition = GlobalPosition + teleportMenuOffset - menu.PivotOffset;
    }
}
