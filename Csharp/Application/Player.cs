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

    private bool _setting_time;

    public static bool muted;
    private double muted_volume;

    public static Player instance;

    public const float MINIMUM_VOLUME = -50.0f, MAXIUMUM_VOLUME = 0.0f, MUTED_VOLUME = -80.0f, VOLUME_STEP = 0.01f;

    public override void _Ready() {
        instance = this;

        loop_button.ButtonUp += SetLoop;
        shuffle_button.ButtonUp += SetShuffle;
        play_button.ButtonUp += Globals.main.FlipPlayingState;

        next_button.ButtonUp += () => Move(1);
        back_button.ButtonUp += () => Move(-1);

        Globals.main.OnLoadTrack += UpdateDisplayedInfo;

        progress_slider.DragEnded += _SetTime;
        progress_slider.DragStarted += () => _setting_time = true;

        Globals.main.OnPlayingChanged += _SetPlayIcon;

        _SetPlayIcon(false);

        mute_button.ButtonUp += ToggleMute;

        _SetMuteIcon();

        volume_slider.MinValue = MINIMUM_VOLUME;
        volume_slider.MaxValue = MAXIUMUM_VOLUME;
        volume_slider.Step = VOLUME_STEP;

        volume_slider.DragStarted += Unmute;

        CallDeferred("_SetShuffleIcon");
    }

    private void _SetTime(bool value_changed) {
        if(value_changed) {
            float time = (float)progress_slider.Value;
            SetTime(time);
        }
        
        _setting_time = false;
    }

    private void _SetShuffleIcon() {
        Color color = Globals.main.shuffled ? ApplicationManager.GetThemeColor("enabled_font_color") : Colors.White;

        shuffle_button.AddThemeColorOverride("icon_normal_color", color);
        shuffle_button.AddThemeColorOverride("icon_focus_color", color);
        shuffle_button.AddThemeColorOverride("icon_pressed_color", color);
        shuffle_button.AddThemeColorOverride("icon_hover_color", color);
        shuffle_button.AddThemeColorOverride("icon_hover_pressed_color", color);
    }

    private void _SetLoopIcon() {
        Color color = Globals.main.loop ? ApplicationManager.GetThemeColor("enabled_font_color") : Colors.White;
        
        loop_button.AddThemeColorOverride("icon_normal_color", color);
        loop_button.AddThemeColorOverride("icon_focus_color", color);
        loop_button.AddThemeColorOverride("icon_pressed_color", color);
        loop_button.AddThemeColorOverride("icon_hover_color", color);
        loop_button.AddThemeColorOverride("icon_hover_pressed_color", color);
    }

    private void _SetMuteIcon() { 
        mute_button.Icon = ApplicationManager.GetThemeIcon(muted ? "unmute_icon" : "mute_icon");
    }

    private void _SetPlayIcon(bool playing) { 
        play_button.Icon = ApplicationManager.GetThemeIcon(!playing ? (!Globals.main.IsTrackAvailable() ? "no_play_icon" : "play_icon") : "pause_icon"); 
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

        instance.muted_volume = Mathf.Max(instance.volume_slider.Value, MINIMUM_VOLUME);

        if (Globals.main.audio_player.VolumeDb == MUTED_VOLUME + 1) {
            muted = false;

            instance._SetMuteIcon();
            instance.volume_slider.Value = instance.muted_volume;
        }
        else {
            Globals.main.audio_player.VolumeDb = MUTED_VOLUME;
            instance.volume_slider.Value = MINIMUM_VOLUME;
        }

        AudioServer.SetBusMute(0, muted);

        instance._SetMuteIcon();
    }

    public static void Unmute() {
        muted = false;

        instance.volume_slider.Value = instance.muted_volume;
        
        SetVolume((float)instance.muted_volume);

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

        Globals.main.audio_player.VolumeDb = value;
    }

    public static void SetShuffle() {
        Globals.main.shuffled = !Globals.main.shuffled;

        Globals.main.offset = Globals.main.track_index;
        Globals.main.shuffle_index = Globals.main.track_index;

        instance._SetShuffleIcon();
    }

    public static void SetLoop() {
        Globals.main.loop = !Globals.main.loop;
        
        instance._SetLoopIcon();
    }

    public static void Move(int by) {
        Globals.main.MoveTrack(by);
    }

    public void UpdateDisplayedInfo() {
        float track_length;

        if (Globals.main.IsTrackAvailable()) {
            track_name_label.Text = Tools.GetMediaTitle(Globals.main.track);
            track_artist_label.Text = Metadata.GetArtist(Globals.main.track);

            if (Globals.main.playlist.type == Playlist.PlaylistType.Album) {                
                track_cover_texture_rect.Texture = Globals.main.playlist.cover_texture;
            }
            else {
                track_cover_texture_rect.Texture = Tools.GetMediaCover(Globals.main.track);
            }

            track_length = Metadata.GetTotalTime(Globals.main.track);

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

        Globals.discord.UpdateTrack();
    }

    public static void SetTime(float value) {
        Globals.main.time = value;

        if (Globals.main.playing) {
            Globals.main.audio_player.Play(Globals.main.time);

            if (Globals.main.video_player.Stream != null) {
                Globals.main.video_player.StreamPosition = Globals.main.time;
            }
        }
    }

    public override void _Process(double delta) {
        if(!ApplicationManager.is_user_typing && ApplicationManager.currently_focused_window == GetTree().Root) {
            if(Input.IsActionJustPressed("play")) Globals.main.FlipPlayingState();
            else if (Input.IsActionJustPressed("next")) Globals.main.MoveTrack(1);
            else if (Input.IsActionJustPressed("back")) Globals.main.MoveTrack(-1);
        }

        current_time_label.Text = Tools.SecondsToTimestamp(Globals.main.time);

        if (!_setting_time) {
            progress_slider.Value = Globals.main.time;
        }
        
        SetVolume((float)(volume_slider.Value != MINIMUM_VOLUME ? volume_slider.Value : MUTED_VOLUME));
    }
}
