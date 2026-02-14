using Godot;

public partial class Player : Node
{
    [Export] public Button play, next, back, loop, shuffle;
    [Export] public Slider progress;
    [Export] public Label current_time, total_time, song_name, song_artist;
    [Export] public TextureRect song_cover;
    [Export] public SubViewport background_subviewport;
    [Export] public Slider volume_slider;
    [Export] public Button mute_button;
    [Export] public ColorRect background_color_rect;

    Color background_color;

    bool can_set_time;

    public bool interrupted, muted;
    public double muted_volume;

    Texture2D playlist_icon;
    int playlist_icon_index = -1;

    public override void _Ready()
	{
        loop.ButtonUp += SetLoop;
        shuffle.ButtonUp += SetShuffle;
        play.ButtonUp += Globals.main.Play;

        next.ButtonUp += () => Move(1);
        back.ButtonUp += () => Move(-1);

        Globals.main.OnLoadSong += OnLoadSong;
        Globals.main.OnLoadPlaylist += ApplyPlaylistSettings;

        progress.DragEnded += SetTime;
        progress.DragStarted += () => can_set_time = true;

        Globals.main.OnPlay += SetPlayIcon;
        play.Icon = Globals.play_texture;

        mute_button.ButtonUp += MuteVolume;
        volume_slider.DragStarted += VolumeUnmute;

        SetMuteTexture();

        CallDeferred("SetShuffleTexture");
    }

    public void SetShuffleTexture() => shuffle.Icon = Globals.main.shuffled ? Globals.shuffle_on_texture : Globals.shuffle_off_texture;
    public void SetMuteTexture() => mute_button.Icon = muted ? Globals.unmute_texture : Globals.mute_texture;

    public void MuteVolume()
    {
        muted = !muted;
        SetMuteTexture();
        if (muted)
        {
            muted_volume = Mathf.Max(volume_slider.Value, -49);
            if (Globals.main.audio_player.VolumeDb == -80f)
            {
                muted = false;
                SetMuteTexture();
                volume_slider.Value = muted_volume;
            }
            else
            {
                Globals.main.audio_player.VolumeDb = -80f;
                volume_slider.Value = -50;
            }
        }
        else
        {
            volume_slider.Value = muted_volume;
        }
    }

    public void VolumeUnmute()
    {
        muted = false;
        SetMuteTexture();
    }

    public bool Interrupt()
    {
        if(!interrupted)
        {
            if (Globals.main.playing) Globals.main.Play();
            interrupted = true;
            return true;
        }
        return false;
    }

    public void SetShuffle()
    {
        Globals.main.shuffled = !Globals.main.shuffled;

        Globals.main.offset = Globals.main.song_index;
        Globals.main.shuffle_index = Globals.main.song_index;

        shuffle.Icon = Globals.main.shuffled ? Globals.shuffle_on_texture : Globals.shuffle_off_texture;
    }

    public void SetLoop()
    {
        Globals.main.loop = !Globals.main.loop;
        loop.Icon = Globals.main.loop ? Globals.loop_on_texture : Globals.loop_off_texture;
    }

    public void SetPlayIcon(bool playing) => play.Icon = !playing || !Globals.main.SongAvailable() ? Globals.play_texture : Globals.pause_texture;

    public void Move(int by)
    {
        if (!interrupted)
        {
            Globals.main.MoveSong(by);
        }
    }

    public void OnLoadSong()
    {
        if (Globals.main.SongAvailable())
        {
            string name = Tools.GetMediaTitle(Globals.main.song);
            song_name.Text = name;
            song_name.TooltipText = name;

            string artist = Metadata.GetArtist(Globals.main.song);
            song_artist.Text = artist;
            song_artist.TooltipText = artist;

            Texture2D cover = ConvertToGodot.GetCover(Globals.main.song);
            song_cover.Texture = cover;

            if (cover == Globals.default_cover && Globals.main.playlist.type == Playlist.PlaylistType.Album)
            {
                if(playlist_icon == null || playlist_icon_index != Globals.main.playlist_index)
                {
                    playlist_icon = ConvertToGodot.LoadImage(Globals.main.playlist.cover) ?? Globals.default_cover;
                    playlist_icon_index = Globals.main.playlist_index;
                }
                
                song_cover.Texture = playlist_icon;
            }
            else
            {
                song_cover.Texture = cover;
            }
            
            Texture2D background_texture = Globals.main.playlist.custom_info.background_path != null ? ConvertToGodot.LoadImage(Globals.main.playlist.custom_info.background_path) ?? cover : song_cover.Texture;

            background_subviewport.Set("target_texture", background_texture);

            total_time.Text = Tools.SecondsToTimestamp(Metadata.GetTotalTime(Globals.main.song));
            progress.MaxValue = Globals.main.audio_player.Stream.GetLength();
            progress.Editable = true;
        }
        else
        {
            song_name.Text = "No song playing";
            song_name.TooltipText = "";
            song_artist.Text = "No artist";
            song_artist.TooltipText = "";
            background_color = new Color(0, 0, 0, 0);

            total_time.Text = "0:00";
            progress.MaxValue = 1;
            progress.Value = 0;
            progress.Editable = false;

            song_cover.Texture = Globals.default_cover;
            background_subviewport.Set("target_texture", Globals.default_cover_highres);
        }

        Globals.discord.UpdateSong();
    }

    public void SetTime(bool value)
    {
        Globals.main.time = (float)progress.Value;
        if (Globals.main.playing)
        {
            Globals.main.audio_player.Play(Globals.main.time);

            if (Globals.main.video_player.Stream != null)
            {
                Globals.main.video_player.StreamPosition = Globals.main.time;
            }
        }
        
        can_set_time = false;
    }
    public void ApplyPlaylistSettings()
    {
        if(Globals.main.playlist == null)
            return;

        background_color = Globals.main.playlist.custom_info.overlay_color != null ? Color.FromHtml(Globals.main.playlist.custom_info.overlay_color) : new Color(0.0f, 0.0f, 0.0f, 0.0f);
    }

    public override void _Process(double delta)
	{
        if(!interrupted)
        {
            if(Input.IsActionJustPressed("play")) Globals.main.Play();
            else if (Input.IsActionJustPressed("next")) Move(1);
            else if (Input.IsActionJustPressed("back")) Move(-1);
        }

        if (Globals.main.audio_player.Stream != null) current_time.Text = Tools.SecondsToTimestamp(Globals.main.time);

        if (!can_set_time)
        {
            progress.Value = Globals.main.time;
        }
        else if (!Globals.main.playing)
        {
            Globals.main.time = (float)progress.Value;
        }

        if (Globals.main.SongAvailable())
        {
            float max = 0.65f;
            background_color_rect.Color = background_color_rect.Color.Lerp(background_color.Clamp(new Color(), new Color(max, max, max, max)), (float)delta * 2f);

            if (!muted)
            {
                Globals.main.audio_player.VolumeDb = (float)(volume_slider.Value != -50 ? volume_slider.Value : -80);
                mute_button.Icon = Globals.main.audio_player.VolumeDb == -80 ? Globals.unmute_texture : Globals.mute_texture;
            }
        }
    }
}
