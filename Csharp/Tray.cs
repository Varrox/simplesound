using Godot;

public partial class Tray : StatusIndicator
{
	[Export] public StreamerButton streamer_button;

	const int EXIT_ID = 0, STREAMER_MODE_ID = 1;

	readonly bool tray_supported = DisplayServer.HasFeature(DisplayServer.Feature.StatusIndicator);

	private PopupMenu popup_menu;

	public override void _Ready() {
		if (tray_supported) {
			popup_menu = (PopupMenu)GetNode(Menu);

			AddPopupItems();

			popup_menu.IdPressed += IdPressed;
		}
		else 
			QueueFree(); // No need for this node if the feature doesn't exist.
	}

    public void AddPopupItems() {
		popup_menu.AddItem($"{(streamer_button.enabled ? "Close" : "Open")} Streamer Window", STREAMER_MODE_ID);
		popup_menu.AddSeparator();
		popup_menu.AddItem("Exit", EXIT_ID);
	}

	private void IdPressed(long id) {
		switch (id) {
			case STREAMER_MODE_ID:
				streamer_button.Toggle();
				popup_menu.SetItemText(0, $"{(streamer_button.enabled ? "Close" : "Open")} Streamer Window");
				break;
			case EXIT_ID:
				ApplicationManager.Quit();
				break;
		}
	}
}