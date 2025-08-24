using Godot;

public partial class Player : Node
{
    [Export] public Button Play, Next, Back, Loop, Shuffle;
    [Export] public Texture2D LoopOn, LoopOff, ShuffleOn, ShuffleOff, Mute, Unmute;
    [Export] public Slider Progress;
    [Export] public Label CurrentTime, TotalTime, SongName, SongArtist;
    [Export] public TextureRect SongCover;
    [Export] public SubViewport BackgroundImage;
    [Export] public Slider VolumeSlider;
    [Export] public Button MuteButton;
    [Export] public ColorRect backgroundColor;

    Color bc;

    bool canSetTime;

    public bool interrupted, muted;
    public double muted_volume;

    Texture2D playlistIcon;
    int playlistIconIndex = -1;

    public override void _Ready()
	{
        Loop.ButtonUp += SetLoop;
        Shuffle.ButtonUp += SetShuffle;
        Play.ButtonUp += Globals.main.Play;

        Next.ButtonUp += () => Move(1);
        Back.ButtonUp += () => Move(-1);

        Globals.main.OnLoadSong += OnLoadSong;
        Globals.main.OnChangePlaylist += OnChangePlaylist;

        Progress.DragEnded += SetTime;
        Progress.DragStarted += () => canSetTime = true;

        Globals.main.OnPlay += PlayIcon;
        Play.Icon = Globals.play_texture;

        MuteButton.ButtonUp += MuteVolume;
        VolumeSlider.DragStarted += VolumeUnmute;

        MuteButton.Icon = muted ? Unmute : Mute;
    }

    public void MuteVolume()
    {
        muted = !muted;
        MuteButton.Icon = muted ? Unmute : Mute;
        if (muted)
        {
            muted_volume = Mathf.Max(VolumeSlider.Value, -49);
            if (Globals.main.player.VolumeDb == -80f)
            {
                muted = false;
                MuteButton.Icon = muted ? Unmute : Mute;
                VolumeSlider.Value = muted_volume;
            }
            else
            {
                Globals.main.player.VolumeDb = -80f;
                VolumeSlider.Value = -50;
            }
        }
        else
        {
            VolumeSlider.Value = muted_volume;
        }
    }

    public void VolumeUnmute()
    {
        muted = false;
        MuteButton.Icon = muted ? Unmute : Mute;
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
        Globals.main.random = !Globals.main.random;

        Globals.main.offset = Globals.main.currentSong;
        Globals.main.shuffleIndex = Globals.main.currentSong;

        Shuffle.Icon = Globals.main.random ? ShuffleOn : ShuffleOff;
    }

    public void SetLoop()
    {
        Globals.main.loop = !Globals.main.loop;
        Loop.Icon = Globals.main.loop ? LoopOn : LoopOff;
    }

    public void PlayIcon(bool playing)
    {
        Play.Icon = !playing || !Globals.main.CanPlay() ? Globals.play_texture : Globals.pause_texture;
    }

    public void Move(int by)
    {
        if (!interrupted)
        {
            Globals.main.MoveSong(by);
        }
    }

    public void OnLoadSong()
    {
        if (Globals.main.CanPlay())
        {
            string name = Tools.GetMediaTitle(Globals.main.song);
            SongName.Text = name;
            SongName.TooltipText = name;

            string artist = Metadata.GetArtist(Globals.main.song);
            SongArtist.Text = artist;
            SongArtist.TooltipText = artist;

            Texture2D cover = ConvertToGodot.GetCover(Globals.main.song, out bool failed);

            if (failed && Globals.main.playlist.Type == Playlist.PlaylistType.Album)
            {
                if(playlistIcon == null || playlistIconIndex != Globals.main.currentPlaylist)
                {
                    playlistIcon = ConvertToGodot.LoadImage(Globals.main.playlist.Cover, ref Globals.default_cover);
                    playlistIconIndex = Globals.main.currentPlaylist;
                }
                
                SongCover.Texture = playlistIcon;
            }
            else
            {
                SongCover.Texture = cover;
            }

            Texture2D background_texture = Globals.main.playlist.customInfo.backgroundPath != null ? ConvertToGodot.LoadImage(Globals.main.playlist.customInfo.backgroundPath, ref cover) : SongCover.Texture;

            BackgroundImage.Set("target_texture", background_texture);

            TotalTime.Text = Tools.SecondsToTimestamp(Metadata.GetTotalTime(Globals.main.song));
            Progress.MaxValue = Globals.main.player.Stream.GetLength();
            Progress.Editable = true;
        }
        else
        {
            SongName.Text = "No song playing";
            SongName.TooltipText = "";
            SongArtist.Text = "No artist";
            SongArtist.TooltipText = "";
            bc = new Color(0, 0, 0, 0);

            TotalTime.Text = "0:00";
            Progress.MaxValue = 1;
            Progress.Value = 0;
            Progress.Editable = false;

            SongCover.Texture = Globals.default_cover;
            BackgroundImage.Set("target_texture", Globals.default_cover_highres);
        }
    }

    public void SetTime(bool value)
    {
        Globals.main.time = (float)Progress.Value;
        if (Globals.main.playing) Globals.main.player.Play(Globals.main.time);
        canSetTime = false;
    }

    public void OnChangePlaylist()
    {
        if (Globals.main.playlist.customInfo.overlayColor != null)
        {
            bc = ConvertToGodot.GetColor(Globals.main.playlist.customInfo.overlayColor);
        }
        else
        {
            bc = new Color();
        }

        Globals.main.player.PitchScale = Mathf.Clamp(Globals.main.playlist.customInfo.speed, 0.01f, 4f);

        var effect = AudioServer.GetBusEffect(0, 0) as AudioEffectReverb;
        effect.RoomSize = Globals.main.playlist.customInfo.reverb;
        effect.Wet = Globals.main.playlist.customInfo.reverb / 100;
    }

    public override void _Process(double delta)
	{
        if(!interrupted)
        {
            if(Input.IsActionJustPressed("play")) Globals.main.Play();
            else if (Input.IsActionJustPressed("next")) Move(1);
            else if (Input.IsActionJustPressed("back")) Move(-1);
        }

        if (Globals.main.player.Stream != null) CurrentTime.Text = Tools.SecondsToTimestamp(Globals.main.time);

        if (!canSetTime)
        {
            Progress.Value = Globals.main.time;
        }
        else if (!Globals.main.playing)
        {
            Globals.main.time = (float)Progress.Value;
        }

        if (Globals.main.CanPlay())
        {
            float max = 0.65f;
            backgroundColor.Color = backgroundColor.Color.Lerp(bc.Clamp(new Color(), new Color(max, max, max, max)), (float)delta * 2f);

            if (!muted)
            {
                Globals.main.player.VolumeDb = (float)(VolumeSlider.Value != -50 ? VolumeSlider.Value : -80) + Globals.main.playlist.customInfo.volume;
                MuteButton.Icon = Globals.main.player.VolumeDb == -80 ? Unmute : Mute;
            }
                
        }
    }
}
