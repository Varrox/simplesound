using Godot;

public partial class ContextMenuButton : MenuButton {
    public override void _Ready() {
		ApplicationManager.ApplyPopupMenuStyle(GetPopup());
        GetPopup().HideOnCheckableItemSelection = false;

        GetPopup().IdPressed += _ItemPressed;
	}

    protected virtual void _ItemPressed(long id) {
        if (GetPopup().IsItemCheckable((int)id)) {
            GetPopup().SetItemChecked((int)id, !GetPopup().IsItemChecked((int)id));
        }
    }
}