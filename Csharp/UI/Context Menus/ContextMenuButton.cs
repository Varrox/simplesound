using Godot;

public partial class ContextMenuButton : MenuButton {
    public override void _Ready() {
		ApplicationManager.ApplyPopupMenuStyle(GetPopup());
        GetPopup().HideOnCheckableItemSelection = false;

        ButtonMask = MouseButtonMask.Left | MouseButtonMask.Right;

        GetPopup().IdPressed += _ItemPressed;
	}

    protected virtual void _ItemPressed(long id) {
        if (GetPopup().IsItemCheckable((int)id)) {
            GetPopup().SetItemChecked((int)id, !GetPopup().IsItemChecked((int)id));
        }
    }

    public void PopupAtMouse() {
        GetPopup().Hide();
        GetPopup().Position = Vector2I.Zero;

        Vector2I popup_point = (Vector2I)GetPopup().GetMousePosition();
        Rect2I r = new Rect2I(popup_point, (Vector2I)GetPopup().GetContentsMinimumSize());
        r.Size = r.Size.Min(GetPopup().GetMaxSize());

        GetPopup().Popup(r);
    }
}