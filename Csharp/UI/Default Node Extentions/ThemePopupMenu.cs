using Godot;

public partial class ThemePopupMenu : PopupMenu {
    public override void _Ready() {
        AddThemeStyleboxOverride("panel", ApplicationManager.theme.GetStylebox("context_menu_panel_stylebox", Constants.THEME_TYPE));
        
        int margin = ApplicationManager.theme.GetConstant("context_menu_margin_constant", Constants.THEME_TYPE);

        AddThemeConstantOverride("v_separation", margin);
        AddThemeConstantOverride("h_separation", margin);

        PopupHide += QueueFree;
    }
}