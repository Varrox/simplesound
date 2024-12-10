using Godot;

public partial class ContextMenu : Button
{
    [Export] public Control menu;
    [Export] public bool teleportMenu;
    [Export] public Vector2 teleportMenuOffset;
    bool menuOpen;
    public override void _Ready()
    {
        ButtonDown += ToggleMenu;
    }

    public override void _Process(double delta)
    {
        if (menuOpen && (Input.IsMouseButtonPressed(MouseButton.Left) || Input.IsMouseButtonPressed(MouseButton.Right) || Input.IsMouseButtonPressed(MouseButton.Middle)))
        {
            if (isMouseInRect(menu) && !IsHovered())
            {
                CloseMenu();
            }
        }
    }

    public static bool isMouseInRect(Control control)
    {
        Vector2I pos = DisplayServer.MouseGetPosition() - DisplayServer.WindowGetPosition();
        Rect2 rect = control.GetGlobalRect();
        Vector2 rpos = rect.GetCenter();
        bool x = pos.X > rpos.X + Mathf.Abs(rect.Size.X / 2) || pos.X < rpos.X - Mathf.Abs(rect.Size.X / 2);
        bool y = pos.Y > rpos.Y + Mathf.Abs(rect.Size.Y / 2) || pos.Y < rpos.Y - Mathf.Abs(rect.Size.Y / 2);
        return x || y;
    }

    public void ToggleMenu()
    {
        menuOpen = !menuOpen;
        if (menuOpen && teleportMenu) TeleportMenu();
        menu.Visible = menuOpen;
    }

    public void CloseMenu()
    {
        menuOpen = false;
        menu.Visible = false;
    }

    public void OpenMenu()
    {
        menuOpen = true;
        TeleportMenu();
        menu.Visible = true;
    }

    public void TeleportMenu()
    {
        menu.GlobalPosition = GlobalPosition + teleportMenuOffset;
    }
}
