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

    bool can_set_time;

    public bool interrupted, muted;
    public double muted_volume;

    public override void _Ready() {
        loop_button.ButtonUp += SetLoop;
        shuffle_button.ButtonUp += SetShuffle;
        play_button.ButtonUp += Globals.main.FlipPlayingState;

        next_button.ButtonUp += () => Move(1);
        back_button.ButtonUp += () => Move(-1);

        Globals.main.OnLoadTrack += OnLoadTrack;
        Globals.main.OnLoadPlaylist += ApplyPlaylistSettings;

        progress_slider.DragEnded += SetTime;
        progress_slider.DragStarted += () => can_set_time = true;

        Globals.main.OnPlayingChanged += SetPlayIcon;
        play_button.Icon = ApplicationManager.theme.GetIcon("play_icon", Constants.THEME_TYPE);

        mute_button.ButtonUp += MuteVolume;
        volume_slider.DragStarted += VolumeUnmute;

        SetMuteTexture();

        CallDeferred("SetShuffleIndicator");
    }

    public void SetShuffleIndicator() {
        Color color = Globals.main.shuffled ? ApplicationManager.theme.GetColor("enabled_font_color", Constants.THEME_TYPE) : Colors.White;
        shuffle_button.AddThemeColorOverride("icon_normal_color", color);
        shuffle_button.AddThemeColorOverride("icon_focus_color", color);
        shuffle_button.AddThemeColorOverride("icon_pressed_color", color);
        shuffle_button.AddThemeColorOverride("icon_hover_color", color);
        shuffle_button.AddThemeColorOverride("icon_hover_pressed_color", color);
    }

    public void SetMuteTexture() => mute_button.Icon = ApplicationManager.theme.GetIcon(muted ? "unmute_icon" : "mute_icon", Constants.THEME_TYPE);

    public void MuteVolume() {
        muted = !muted;
        SetMuteTexture();
        if (muted) {
            muted_volume = Mathf.Max(volume_slider.Value, -49);
            if (Globals.main.audio_player.VolumeDb == -80f) {
                muted = false;
                SetMuteTexture();
                volume_slider.Value = muted_volume;
            }
            else {
                Globals.main.audio_player.VolumeDb = -80f;
                volume_slider.Value = -50;
            }
        }
        else volume_slider.Value = muted_volume;
    }

    public void VolumeUnmute() {
        muted = false;
        SetMuteTexture();
    }

    public bool Interrupt() {
        if (interrupted)
            return false;
        
        Globals.main.Pause();
        interrupted = true;
        return true;
    }

    public void SetShuffle() {
        Globals.main.shuffled = !Globals.main.shuffled;

        Globals.main.offset = Globals.main.track_index;
        Globals.main.shuffle_index = Globals.main.track_index;

        SetShuffleIndicator();
    }

    public void SetLoop() {
        Globals.main.loop = !Globals.main.loop;
        
        Color color = Globals.main.loop ? ApplicationManager.theme.GetColor("enabled_font_color", Constants.THEME_TYPE) : Colors.White;
        loop_button.AddThemeColorOverride("icon_normal_color", color);
        loop_button.AddThemeColorOverride("icon_focus_color", color);
        loop_button.AddThemeColorOverride("icon_pressed_color", color);
        loop_button.AddThemeColorOverride("icon_hover_color", color);
        loop_button.AddThemeColorOverride("icon_hover_pressed_color", color);
    }

    public void SetPlayIcon(bool playing) { 
        play_button.Icon = ApplicationManager.theme.GetIcon(!playing || !Globals.main.IsTrackAvailable() ? "play_icon" : "pause_icon", Constants.THEME_TYPE); 
    }

    public void Move(int by) {
        if (!interrupted) Globals.main.MoveTrack(by);
    }

    public void OnLoadTrack() {
        Texture2D default_cover_icon = ApplicationManager.theme.GetIcon("default_cover_icon", Constants.THEME_TYPE);

        if (Globals.main.IsTrackAvailable()) {
            string name = Tools.GetMediaTitle(Globals.main.track);
            track_name_label.Text = name;
            track_name_label.TooltipText = name;

            string artist = Metadata.GetArtist(Globals.main.track);
            track_artist_label.Text = artist;
            track_artist_label.TooltipText = artist;

            if (Globals.main.playlist.type == Playlist.PlaylistType.Album) {                
                track_cover_texture_rect.Texture = Globals.main.playlist.cover_texture;
            }
            else {
                track_cover_texture_rect.Texture = Tools.GetMediaCover(Globals.main.track);
            }

            if (track_cover_texture_rect.Texture == null) {
                track_cover_texture_rect.Texture = default_cover_icon;
            }

            background_subviewport.Set("target_texture", track_cover_texture_rect.Texture);

            total_time_label.Text = Tools.SecondsToTimestamp(Metadata.GetTotalTime(Globals.main.track));
            progress_slider.MaxValue = Globals.main.audio_player.Stream.GetLength();
            progress_slider.Editable = true;
        }
        else {
            track_name_label.Text = "No track playing";
            track_name_label.TooltipText = "";
            track_artist_label.Text = "No artist";
            track_artist_label.TooltipText = "";

            total_time_label.Text = "0:00";
            progress_slider.MaxValue = 1;
            progress_slider.Value = 0;
            progress_slider.Editable = false;

            track_cover_texture_rect.Texture = default_cover_icon;
            background_subviewport.Set("target_texture", default_cover_icon);
        }

        Globals.discord.UpdateTrack();
    }

    public void SetTime(bool value) {
        Globals.main.time = (float)progress_slider.Value;
        if (Globals.main.playing) {
            Globals.main.audio_player.Play(Globals.main.time);

            if (Globals.main.video_player.Stream != null) {
                Globals.main.video_player.StreamPosition = Globals.main.time;
            }
        }
        
        can_set_time = false;
    }
    public void ApplyPlaylistSettings() {
        if(Globals.main.playlist == null) return;
    }

    public override void _Process(double delta) {
        if(!ApplicationManager.is_user_typing && ApplicationManager.currently_focused_window == GetTree().Root) {
            if(Input.IsActionJustPressed("play")) Globals.main.FlipPlayingState();
            else if (Input.IsActionJustPressed("next")) Globals.main.MoveTrack(1);
            else if (Input.IsActionJustPressed("back")) Globals.main.MoveTrack(-1);
        }

        if (Globals.main.audio_player.Stream != null) current_time_label.Text = Tools.SecondsToTimestamp(Globals.main.time);

        if (!can_set_time) {
            progress_slider.Value = Globals.main.time;
        }
        else if (!Globals.main.playing) {
            Globals.main.time = (float)progress_slider.Value;
        }

        if (!muted) {
            Globals.main.audio_player.VolumeDb = (float)(volume_slider.Value != -50 ? volume_slider.Value : -80);
            mute_button.Icon = ApplicationManager.theme.GetIcon(Globals.main.audio_player.VolumeDb == -80 ? "unmute_icon" : "mute_icon", Constants.THEME_TYPE);
        }

        AudioServer.SetBusMute(0, Globals.main.audio_player.VolumeDb == -80);
    }
}
