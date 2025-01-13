using Godot;

public partial class Player : Node
{
    [Export] public Main MainController;
    [Export] public Button Play, Next, Back, Loop, Shuffle;
    [Export] public Texture2D PlayIcon, PauseIcon, LoopOn, LoopOff, ShuffleOn, ShuffleOff;
    [Export] public Slider Progress;
    [Export] public Label CurrentTime, TotalTime, SongName, SongArtist;
    [Export] public TextureRect SongCover, BackgroundImage;
    [Export] public Slider VolumeSlider;
    [Export] public ColorRect backgroundColor;

    [Export] public Button EditAttributes;
    [Export] public AttributeEditor AttributeEditor;

    Color bc;

    bool canSetTime, attributesBeingedited, spacepressed;

    public override void _Ready()
	{
        Loop.ButtonDown += setLoop;
        Shuffle.ButtonDown += SetShuffle;
        Play.ButtonDown += MainController.Play;
        Next.ButtonDown += () => move(1);
        Back.ButtonDown += () => move(-1);
        MainController.OnLoadSong += onLoadSong;
        Progress.DragEnded += setTime;
        Progress.DragStarted += () => canSetTime = true;
        EditAttributes.ButtonDown += editAttributes;
        AttributeEditor.onSubmitdata += submitmeta;

        GetTree().Root.MinSize = new Vector2I(850, 350);
        AttributeEditor.AttributeWindow.Hide();
        MainController.OnPlay += Playicon;
        Playicon(false);
    }

    public void editAttributes()
    {
        if(!attributesBeingedited)
        {
            AttributeEditor.open(SongName.Text, SongArtist.Text, Metadata.IsExplicit(MainController.song));
            attributesBeingedited = true;

            if (MainController.playing) MainController.Play();
        }
    }

    public void SetShuffle()
    {
        MainController.random = !MainController.random;
        MainController.shuffleIndex = MainController.currentSong;
        Shuffle.Icon = MainController.random ? ShuffleOn : ShuffleOff;
    }

    public void setLoop()
    {
        MainController.loop = !MainController.loop;
        Loop.Icon = MainController.loop ? LoopOn : LoopOff;
    }

    public void Playicon(bool playing)
    {
        Play.Icon = !playing ? PlayIcon : PauseIcon;
    }

    public void move(int by)
    {
        if (!attributesBeingedited)
        {
            MainController.MoveSong(by);
        }
    }

    public void onLoadSong()
    {
        if (MainController.playlist)
        {
            string name = SaveSystem.GetName(MainController.song);
            SongName.Text = name;
            SongName.TooltipText = name;
            string artist = Metadata.GetArtist(MainController.song);
            SongArtist.Text = artist;
            SongArtist.TooltipText = artist;
            Texture2D cover = ConvertToGodot.getCover(MainController.song);
            SongCover.Texture = cover;
            if (MainController.playlist.backgroundPath != null) BackgroundImage.Texture = ConvertToGodot.LoadImage(MainController.playlist.backgroundPath, ref cover);
            else BackgroundImage.Texture = cover;
        }
        else
        {
            SongName.Text = "No song playing";
            SongName.TooltipText = "";
            SongArtist.Text = "No artist";
            SongArtist.TooltipText = "";
            bc = new Color(0, 0, 0, 0);
        }

        TotalTime.Text = SaveSystem.GetTimeFromSeconds(Metadata.GetTotalTime(MainController.song));
        Progress.MaxValue = MainController.player.Stream.GetLength();
    }

    public void setTime(bool value)
    {
        MainController.time = (float)Progress.Value;
        if (MainController.playing) MainController.player.Play(MainController.time);
        canSetTime = false;
    }

    public void submitmeta()
    {
        MainController.EditMeta(AttributeEditor.songname, AttributeEditor.artist, AttributeEditor.coverpath, AttributeEditor.explicitLyrics);
        onLoadSong();
        attributesBeingedited = false;
        MainController.songsVisualizer.UpdateSong(MainController.currentSong, AttributeEditor.songname, AttributeEditor.artist, TotalTime.Text, AttributeEditor.explicitLyrics, SongCover.Texture);
    }

    public override void _Process(double delta)
	{
        if(!attributesBeingedited)
        {
            if (Input.IsKeyPressed(Key.Space) && !spacepressed)
            {
                MainController.Play();
                spacepressed = true;
            }
            else if (!Input.IsKeyPressed(Key.Space) && spacepressed) spacepressed = false;
        }

        if (MainController.player.Stream != null) CurrentTime.Text = SaveSystem.GetTimeFromSeconds(MainController.time);

        if (!canSetTime)
        {
            Progress.Value = MainController.time;
        }
        else if (!MainController.playing)
        {
            MainController.time = (float)Progress.Value;
        }

        float max = 0.65f;

        if(MainController.playlist.overlayColor != null)
        {
            bc = ConvertToGodot.GetColor(MainController.playlist.overlayColor);
        }
        else
        {
            bc = new Color();
        }

        backgroundColor.Color = backgroundColor.Color.Lerp(bc.Clamp(new Color(), new Color(max, max, max, max)), (float)delta * 2f);

        MainController.player.VolumeDb = (float)(VolumeSlider.Value != -50 ? VolumeSlider.Value : -80) + MainController.playlist.volume;
        MainController.player.PitchScale = MainController.playlist.speed;
        var effect = AudioServer.GetBusEffect(0, 0) as AudioEffectReverb;
        effect.RoomSize = MainController.playlist.reverb;
        //AudioServer.GetBusEffect();
    }
}
