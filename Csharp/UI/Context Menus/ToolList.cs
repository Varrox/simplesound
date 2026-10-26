using Godot;

public partial class ToolList : ContextMenuButton {
    [Export] public PackedScene streamer_display_template;
    [Export] public TextureRect background_texture_rect;
    [Export] public HBoxContainer text_container;

    private StreamDisplay _streamer_display;

    private ThemePopupMenu _sleep_timer_popup_menu;
    private ContextMenuButton _sleep_timer_button;
    private Timer _sleep_timer;
    
    public const int SEP_1 = 1;

    public const int SLEEP_TIMER_STOP = 0;

    public const int SLEEP_5_MINUTES = 0, SLEEP_10_MINUTES = 1, SLEEP_15_MINUTES = 2, SLEEP_20_MINUTES = 3, SLEEP_30_MINUTES = 4, SLEEP_45_MINUTES = 5, SLEEP_1_HOUR = 6, SLEEP_2_HOURS = 7;

    public const int STREAMER_DISPLAY = 0, SLEEP_TIMER = 2;

    public override void _Ready() {
        base._Ready();

        GetPopup().AddCheckItem("Streamer Mode", STREAMER_DISPLAY);

        GetPopup().AddSeparator("", SEP_1);

        _sleep_timer_popup_menu = new ThemePopupMenu();

        GetPopup().AddSubmenuNodeItem("Sleep Timer", _sleep_timer_popup_menu, SLEEP_TIMER);
        GetPopup().SetItemIcon(SLEEP_TIMER, ApplicationManager.GetThemeIcon("time_icon"));

        _sleep_timer_popup_menu.AddItem("5 minutes", SLEEP_5_MINUTES);
        _sleep_timer_popup_menu.AddItem("10 minutes", SLEEP_10_MINUTES);
        _sleep_timer_popup_menu.AddItem("15 minutes", SLEEP_15_MINUTES);
        _sleep_timer_popup_menu.AddItem("20 minutes", SLEEP_20_MINUTES);
        _sleep_timer_popup_menu.AddItem("30 minutes", SLEEP_30_MINUTES);
        _sleep_timer_popup_menu.AddItem("45 minutes", SLEEP_45_MINUTES);
        _sleep_timer_popup_menu.AddItem("1 hour", SLEEP_1_HOUR);
        _sleep_timer_popup_menu.AddItem("2 hours", SLEEP_2_HOURS);

        _sleep_timer_popup_menu.IdPressed += _SetupSleepTimer;
    }

    protected override void _ItemPressed(long id) {
        base._ItemPressed(id);

        switch(id) {
            case STREAMER_DISPLAY:
                _ToggleStreamDisplay();
                break;
        }
    }

    private void _SetupSleepTimer(long id) {
        float time = 0.0f;

        switch(id) {
            case SLEEP_5_MINUTES:
                time = 60 * 5;
                break;
            case SLEEP_10_MINUTES:
                time = 60 * 10;
                break;
            case SLEEP_15_MINUTES:
                time = 60 * 15;
                break;
            case SLEEP_20_MINUTES:
                time = 60 * 20;
                break;
            case SLEEP_30_MINUTES:
                time = 60 * 30;
                break;
            case SLEEP_45_MINUTES:
                time = 60 * 45;
                break;
            case SLEEP_1_HOUR:
                time = 60 * 60;
                break;
            case SLEEP_2_HOURS:
                time = 60 * 60 * 2;
                break;
        }

        _CreateSleepTimer(time);
    }

    private void _CreateSleepTimer(float time_seconds) {
        // Button

        if (_sleep_timer_button != null) {
            _sleep_timer_button.QueueFree();
            _sleep_timer_button = null;
        }

        _sleep_timer_button = new ContextMenuButton();
        text_container.AddChild(_sleep_timer_button);

        _sleep_timer_button.Flat = false;

        _sleep_timer_button.Icon = ApplicationManager.GetThemeIcon("time_icon");
        _sleep_timer_button.AddThemeConstantOverride("icon_max_width", 20);

        _sleep_timer_button.Text = $"Sleep timer: {Tools.SecondsToTimestamp(time_seconds)}";

        _sleep_timer_button.GetPopup().AddIconItem(ApplicationManager.GetThemeIcon("stop_icon"), "Stop timer", SLEEP_TIMER_STOP);

        _sleep_timer_button.GetPopup().IdPressed += _SleepTimerButtonItemPressed;

        // Timer

        if (_sleep_timer != null) {
            _sleep_timer.QueueFree();
            _sleep_timer = null;
        }

        _sleep_timer = new Timer();
        AddChild(_sleep_timer);

        _sleep_timer.Start(time_seconds);
        _sleep_timer.Timeout += _SleepTimerFinished;
    }

    private void _SleepTimerButtonItemPressed(long id) {
        switch (id) {
            case SLEEP_TIMER_STOP:
                _ClearTimer();
                break;
        }
    }

    private void _ClearTimer() {
        _sleep_timer_button.GetPopup().IdPressed -= _SleepTimerButtonItemPressed;
        _sleep_timer_button.QueueFree();
        _sleep_timer_button = null;

        _sleep_timer.Timeout -= _SleepTimerFinished;
        _sleep_timer.QueueFree();
        _sleep_timer = null;
    }

    private void _SleepTimerFinished() {
        Globals.main.Pause();
        _ClearTimer();
    }

    private void _ToggleStreamDisplay() {
		if (IsStreamDisplayOpen()) { 
			_streamer_display = streamer_display_template.Instantiate() as StreamDisplay;
			GetTree().CurrentScene.AddChild(_streamer_display);

            _streamer_display.background_texture_rect.Texture = background_texture_rect.Texture;
        }
		else {
			_streamer_display.QueueFree();
		}
    }

    public void ToggleStreamDisplay() {
        _ItemPressed(STREAMER_DISPLAY);
    }

    public bool IsStreamDisplayOpen() {
        return GetPopup().IsItemChecked(STREAMER_DISPLAY);
    }

    public override void _Process(double delta) {
        if (_sleep_timer_button != null && _sleep_timer != null) {
            _sleep_timer_button.Text = $"Sleep timer: {Tools.SecondsToTimestamp((float)_sleep_timer.TimeLeft)}";
        }
    }
}
