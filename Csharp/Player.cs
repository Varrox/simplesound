using Godot;

[GlobalClass]
public partial class Player : Node
{
    [ExportGroup("Controls")]
    [Export] public Button play_button, next_button, back_button, loop_button, shuffle_button;

    [Export] public Slider progress_slider, volume_slider;

    [Export] public Button mute_button;

    [ExportGroup("Visuals")]
    [Export] public Label current_time_label, total_time_label, track_name_label, track_artist_label;
    [Export] public TextureRect track_cover_texture_rect;
    [Export] public SubViewport background_subviewport;
    [Export] public ColorRect background_color_rect;

    Color background_color;

    bool can_set_time;

    public bool interrupted, muted;
    public double muted_volume;

    Texture2D playlist_icon;
    int playlist_icon_index = -1;

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
        if (Globals.main.IsTrackAvailable()) {
            string name = Tools.GetMediaTitle(Globals.main.track);
            track_name_label.Text = name;
            track_name_label.TooltipText = name;

            string artist = Metadata.GetArtist(Globals.main.track);
            track_artist_label.Text = artist;
            track_artist_label.TooltipText = artist;

            Texture2D cover = ConvertToGodot.GetMediaCover(Globals.main.track);
            track_cover_texture_rect.Texture = cover;

            Texture2D default_cover_icon = ApplicationManager.theme.GetIcon("default_cover_icon", Constants.THEME_TYPE);

            if (cover == default_cover_icon && Globals.main.playlist.type == Playlist.PlaylistType.Album) {
                if(playlist_icon == null || playlist_icon_index != Globals.main.playlist_index) {
                    playlist_icon = ConvertToGodot.LoadImageFromFile(Globals.main.playlist.cover) ?? default_cover_icon;
                    playlist_icon_index = Globals.main.playlist_index;
                }
                
                track_cover_texture_rect.Texture = playlist_icon;
            }
            else {
                track_cover_texture_rect.Texture = cover;
            }
            
            Texture2D background_texture = Globals.main.playlist.custom_info.background_path != null ? ConvertToGodot.LoadImageFromFile(Globals.main.playlist.custom_info.background_path) ?? cover : track_cover_texture_rect.Texture;

            background_subviewport.Set("target_texture", background_texture);

            total_time_label.Text = Tools.SecondsToTimestamp(Metadata.GetTotalTime(Globals.main.track));
            progress_slider.MaxValue = Globals.main.audio_player.Stream.GetLength();
            progress_slider.Editable = true;
        }
        else {
            track_name_label.Text = "No track playing";
            track_name_label.TooltipText = "";
            track_artist_label.Text = "No artist";
            track_artist_label.TooltipText = "";
            background_color = Colors.Transparent;

            total_time_label.Text = "0:00";
            progress_slider.MaxValue = 1;
            progress_slider.Value = 0;
            progress_slider.Editable = false;

            Texture2D default_cover_icon = ApplicationManager.theme.GetIcon("default_cover_icon", Constants.THEME_TYPE);

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

        background_color = Globals.main.playlist.custom_info.overlay_color != null ? Color.FromHtml(Globals.main.playlist.custom_info.overlay_color) : Colors.Transparent;
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

        if (Globals.main.IsTrackAvailable()) {
            float max = 0.65f;
            background_color_rect.Color = background_color_rect.Color.Lerp(background_color.Clamp(new Color(0f, 0f, 0f, 0f), new Color(max, max, max, max)), (float)delta * 2f);

            if (!muted) {
                Globals.main.audio_player.VolumeDb = (float)(volume_slider.Value != -50 ? volume_slider.Value : -80);
                mute_button.Icon = ApplicationManager.theme.GetIcon(Globals.main.audio_player.VolumeDb == -80 ? "unmute_icon" : "mute_icon", Constants.THEME_TYPE);
            }
        }
    }
}
