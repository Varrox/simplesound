using Godot;

public partial class SourceButton : LinkButton {
	public override void _Ready() {
		Uri = Constants.SOURCE;
		Text = $"{ApplicationManager.SOFTWARE_NAME} {ProjectSettings.GetSetting("application/config/version")}";

		AddThemeColorOverride("font_color", ApplicationManager.GetThemeColor("small_font_color"));

		TooltipText = $"Opens the git repo page to {ApplicationManager.SOFTWARE_NAME}'s source code in browser.";
	}
}
