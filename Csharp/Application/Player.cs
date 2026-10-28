using Godot;

[GlobalClass]
public partial class Player : Node {
    [ExportGroup("Controls")]
    [Export] public Button play_button, next_button, back_button, loop_button, shuffle_button;

    [Export] public Slider progress_slider, volume_slider;

    [Export] public Button mute_button;

    [ExportGroup("Visuals")]
    [Export] public Label current_time_label, total_time_label, track_name_label, track_artist_label;
    [Export] public TextureRect track_cover_texture_rect;
    [Export] public SubViewport background_subviewport;
    [Export] public BlitEffectRect background;

    private bool _setting_time;

    public static bool muted;

    private static double _muted_volume;

    public static Player instance;

    public const float MINIMUM_VOLUME = -50.0f, MAXIUMUM_VOLUME = 0.0f, MUTED_VOLUME = -80.0f, VOLUME_STEP = 0.01f;

    public override void _Ready() {
        instance = this;

        Main.instance.OnLoadTrack += UpdateDisplayedInfo;

        progress_slider.DragEnded += _SetTime;
        progress_slider.DragStarted += () => _setting_time = true;

        // Shuffle

        shuffle_button.ButtonUp += ToggleShuffle;

        CallDeferred("_SetShuffleIcon");

        // Back

        back_button.ButtonUp += () => Main.instance.MoveTrack(-1);

        // Pause / Play

        play_button.ButtonUp += Main.instance.FlipPlayingState;

        Main.instance.OnPlayingChanged += _SetPlayIcon;

        _SetPlayIcon();

        // Next

        next_button.ButtonUp += () => Main.instance.MoveTrack(1);

        // Loop

        loop_button.ButtonUp += ToggleLoop;

        // Mute

        mute_button.ButtonUp += ToggleMute;

        _SetMuteIcon();

        // Volume Slider

        volume_slider.MinValue = MINIMUM_VOLUME;
        volume_slider.MaxValue = MAXIUMUM_VOLUME;
        volume_slider.Step = VOLUME_STEP;

        volume_slider.DragStarted += Unmute;
    }

    private void _SetMutedVolume() {
        _muted_volume = Mathf.Max(volume_slider.Value, MINIMUM_VOLUME + 1);
    }

    private void _SetTime(bool value_changed) {
        if(value_changed) {
            float time = (float)progress_slider.Value;
            SetTime(time);
        }
        
        _setting_time = false;
    }

    private void _SetShuffleIcon() {
        Color color = Main.shuffled ? ApplicationManager.GetThemeColor("enabled_font_color") : Colors.White;

        shuffle_button.AddThemeColorOverride("icon_normal_color", color);
        shuffle_button.AddThemeColorOverride("icon_focus_color", color);
        shuffle_button.AddThemeColorOverride("icon_pressed_color", color);
        shuffle_button.AddThemeColorOverride("icon_hover_color", color);
        shuffle_button.AddThemeColorOverride("icon_hover_pressed_color", color);
    }

    private void _SetLoopIcon() {
        Color color = Main.loop ? ApplicationManager.GetThemeColor("enabled_font_color") : Colors.White;
        
        loop_button.AddThemeColorOverride("icon_normal_color", color);
        loop_button.AddThemeColorOverride("icon_focus_color", color);
        loop_button.AddThemeColorOverride("icon_pressed_color", color);
        loop_button.AddThemeColorOverride("icon_hover_color", color);
        loop_button.AddThemeColorOverride("icon_hover_pressed_color", color);
    }

    private void _SetMuteIcon() { 
        mute_button.Icon = ApplicationManager.GetThemeIcon(muted ? "unmute_icon" : "mute_icon");
    }

    private void _SetPlayIcon() { 
        play_button.Icon = ApplicationManager.GetThemeIcon(!Main.playing ? (!Main.IsTrackAvailable() ? "no_play_icon" : "play_icon") : "pause_icon"); 
    }

    public static void ToggleMute() {
        if (muted) {
            Unmute();
        }
        else {
            Mute();
        }
    }

    public static void Mute() {
        muted = true;

        Main.instance.audio_player.VolumeDb = MUTED_VOLUME;
        instance.volume_slider.Value = MINIMUM_VOLUME;

        AudioServer.SetBusMute(0, muted);

        instance._SetMuteIcon();
    }

    public static void Unmute() {
        muted = false;

        Main.instance.audio_player.VolumeDb = (float)_muted_volume;
        instance.volume_slider.Value = _muted_volume;

        AudioServer.SetBusMute(0, muted);

        instance._SetMuteIcon();
    }

    public static void SetVolume(float value) {
        if(value <= MINIMUM_VOLUME) {
            Mute();
        }
        else if(muted) {
            Unmute();
        }
        else {
            Main.instance.audio_player.VolumeDb = value;
            instance.volume_slider.Value = value;

            instance._SetMutedVolume();
        }
    }

    public static void ToggleShuffle() {
        Main.shuffled = !Main.shuffled;

        Main.offset = Main.track_index;
        Main.shuffle_index = Main.track_index;

        instance._SetShuffleIcon();
    }

    public static void ToggleLoop() {
        Main.loop = !Main.loop;
        
        instance._SetLoopIcon();
    }

    public void UpdateDisplayedInfo() {
        float track_length;

        if (Main.IsTrackAvailable()) {
            track_name_label.Text = Tools.GetMediaTitle(Main.GetCurrentTrack());
            track_artist_label.Text = Metadata.GetArtist(Main.GetCurrentTrack());

            if (Main.GetCurrentPlaylist().type == Playlist.PlaylistType.Album) {                
                track_cover_texture_rect.Texture = Main.GetCurrentPlaylist().cover_texture;
            }
            else {
                track_cover_texture_rect.Texture = Tools.GetMediaCover(Main.GetCurrentTrack());
            }

            track_length = Metadata.GetTotalTime(Main.GetCurrentTrack());

            progress_slider.Editable = true;
        }
        else {
            track_name_label.Text = "No track playing";
            track_artist_label.Text = "No artist";

            track_cover_texture_rect.Texture = null;
            
            track_length = 0.0f;

            progress_slider.Value = 0.0f;
            progress_slider.Editable = false;
        }

        track_name_label.TooltipText = track_name_label.Text;
        track_artist_label.TooltipText = track_artist_label.Text;

        if (track_cover_texture_rect.Texture == null) {
            track_cover_texture_rect.Texture = ApplicationManager.GetThemeIcon("default_cover_icon");
        }

        
        background_subviewport.Set("target_texture", track_cover_texture_rect.Texture);

        total_time_label.Text = Tools.SecondsToTimestamp(track_length);

        progress_slider.MaxValue = track_length;

        _SetPlayIcon();

        Discord.UpdateTrack();
    }

    public static void SetTime(float value) {
        Main.time = value;

        if (Main.playing) {
            Main.instance.audio_player.Play(Main.time);

            if (Main.instance.video_player.Stream != null) {
                Main.instance.video_player.StreamPosition = Main.time;
            }
        }
    }

    public override void _Process(double delta) {
        if(!ApplicationManager.is_user_typing && ApplicationManager.currently_focused_window == GetTree().Root) {
            if(Input.IsActionJustPressed("play")) Main.instance.FlipPlayingState();
            else if (Input.IsActionJustPressed("next")) Main.instance.MoveTrack(1);
            else if (Input.IsActionJustPressed("back")) Main.instance.MoveTrack(-1);
        }

        current_time_label.Text = Tools.SecondsToTimestamp(Main.time);

        if (!_setting_time) {
            progress_slider.Value = Main.time;
        }
        
        SetVolume((float)(volume_slider.Value != MINIMUM_VOLUME ? volume_slider.Value : MUTED_VOLUME));
    }
}
