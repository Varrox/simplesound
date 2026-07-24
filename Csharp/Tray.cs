using Godot;

public partial class Tray : StatusIndicator
{
	const int EXIT_ID = 0;

	readonly bool tray_supported = DisplayServer.HasFeature(DisplayServer.Feature.StatusIndicator);

	public override void _Ready() {
		if (tray_supported) {
			PopupMenu popup_menu = (PopupMenu)GetNode(Menu);

			AddPopupItems(popup_menu);

			popup_menu.IdPressed += IdPressed;
		}
		else 
			QueueFree(); // No need for this node if the feature doesn't exist.
	}

    public void AddPopupItems(PopupMenu popup_menu) {
		popup_menu.AddItem("Exit", EXIT_ID);
	}

	private void IdPressed(long id) {
		switch (id) {
			case EXIT_ID:
				Globals.Quit();
				break;
		}
	}
}