using Godot;

public partial class Player : Node
{
    [Export] public Button Play, Next, Back, Loop, Shuffle;
    [Export] public Texture2D LoopOn, LoopOff, ShuffleOn, ShuffleOff;
    [Export] public Slider Progress;
    [Export] public Label CurrentTime, TotalTime, SongName, SongArtist;
    [Export] public TextureRect SongCover, BackgroundImage;
    [Export] public Slider VolumeSlider;
    [Export] public ColorRect backgroundColor;

    Color bc;

    bool canSetTime;

    public bool interrupted;

    public override void _Ready()
	{
        GetTree().Root.MinSize = new Vector2I(850, 350);

        Loop.ButtonDown += setLoop;
        Shuffle.ButtonDown += SetShuffle;
        Play.ButtonDown += Globals.main.Play;

        Next.ButtonDown += () => move(1);
        Back.ButtonDown += () => move(-1);

        Globals.main.OnLoadSong += onLoadSong;

        Progress.DragEnded += setTime;
        Progress.DragStarted += () => canSetTime = true;

        Globals.main.OnPlay += Playicon;
        Playicon(false);
    }

    public bool interrupt()
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
        Globals.main.shuffleIndex = Globals.main.currentSong;
        Shuffle.Icon = Globals.main.random ? ShuffleOn : ShuffleOff;
    }

    public void setLoop()
    {
        Globals.main.loop = !Globals.main.loop;
        Loop.Icon = Globals.main.loop ? LoopOn : LoopOff;
    }

    public void Playicon(bool playing)
    {
        Play.Icon = !playing || !Globals.main.CanPlay()? Globals.play_texture : Globals.pause_texture;
    }

    public void move(int by)
    {
        if (!interrupted)
        {
            Globals.main.MoveSong(by);
        }
    }

    public void onLoadSong()
    {
        if (Globals.main.CanPlay())
        {
            string name = Tools.GetMediaTitle(Globals.main.song);
            SongName.Text = name;
            SongName.TooltipText = name;
            string artist = Metadata.GetArtist(Globals.main.song);
            SongArtist.Text = artist;
            SongArtist.TooltipText = artist;
            Texture2D cover = ConvertToGodot.GetCover(Globals.main.song);
            SongCover.Texture = cover;
            if (Globals.main.playlist.customInfo.backgroundPath != null) BackgroundImage.Texture = ConvertToGodot.LoadImage(Globals.main.playlist.customInfo.backgroundPath, ref cover);
            else BackgroundImage.Texture = cover;

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
            BackgroundImage.Texture = Globals.default_cover_highres;
        }
    }

    public void setTime(bool value)
    {
        Globals.main.time = (float)Progress.Value;
        if (Globals.main.playing) Globals.main.player.Play(Globals.main.time);
        canSetTime = false;
    }

    public override void _Process(double delta)
	{
        if(!interrupted)
        {
            if(Input.IsActionJustPressed("play")) Globals.main.Play();
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

        if (Globals.main.playlist != null)
        {
            if (Globals.main.playlist.customInfo.overlayColor != null)
            {
                bc = ConvertToGodot.GetColor(Globals.main.playlist.customInfo.overlayColor);
            }
            else
            {
                bc = new Color();
            }

            float max = 0.65f;
            backgroundColor.Color = backgroundColor.Color.Lerp(bc.Clamp(new Color(), new Color(max, max, max, max)), (float)delta * 2f);

            Globals.main.player.VolumeDb = (float)(VolumeSlider.Value != -50 ? VolumeSlider.Value : -80) + Globals.main.playlist.customInfo.volume;
            Globals.main.player.PitchScale = Mathf.Clamp(Globals.main.playlist.customInfo.speed, 0.01f, 4f);

            var effect = AudioServer.GetBusEffect(0, 0) as AudioEffectReverb;
            effect.RoomSize = Globals.main.playlist.customInfo.reverb;
            effect.Wet = Globals.main.playlist.customInfo.reverb / 100;
        }
    }
}
