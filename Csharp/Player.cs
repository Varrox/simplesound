using Godot;

public partial class Player : Node
{
    [Export] public Main MainController;
    [Export] public Node DiscordPresense;
    [Export] public Button Play, Next, Back, Loop;
    [Export] public Texture2D PlayIcon, PauseIcon, LoopOn, LoopOff;
    [Export] public Slider Progress;
    [Export] public Label CurrentTime, TotalTime, SongName, SongArtist;
    [Export] public TextureRect SongCover;
    [Export] public Texture2D DefaultCover;
    [Export] public Slider VolumeSlider;

    [Export] public Button EditAttributes;
    [Export] public AttributeEditor AttributeEditor;

    bool canSetTime, attributesBeingedited;
    float tim = 0;

    public override void _Ready()
	{
        Loop.ButtonDown += setLoop;
        Play.ButtonDown += setPlay;
        Next.ButtonDown += () => move(1);
        Back.ButtonDown += () => move(-1);
        MainController.OnLoadSong += onLoadSong;
        Progress.DragEnded += setTime;
        Progress.DragStarted += () => canSetTime = true;
        EditAttributes.ButtonDown += editAttributes;
        AttributeEditor.onSubmitdata += submitmeta;

        GetTree().Root.MinSize = new Vector2I(850, 350);
        AttributeEditor.AttributeWindow.Hide();

    }

    public void editAttributes()
    {
        if(!attributesBeingedited)
        {
            AttributeEditor.open(SongName.Text, SongArtist.Text);
            attributesBeingedited = true;

            if (MainController.playing) setPlay();
        }
    }

    public void setLoop()
    {
        MainController.loop = !MainController.loop;
        Loop.Icon = MainController.loop ? LoopOn : LoopOff;
    }

    public void setPlay()
    {
        MainController.Play();
        Play.Icon = !MainController.playing ? PlayIcon : PauseIcon;
    }

    public void move(int by)
    {
        if (!attributesBeingedited)
        {
            MainController.MoveSong(by);
            Play.Icon = !MainController.playing ? PlayIcon : PauseIcon;
        }
    }

    public void onLoadSong()
    {
        if (MainController.playlist)
        {
            string name = SaveSystem.GetName(MainController.playlist.songs[MainController.currentSong]);
            SongName.Text = name;
            SongName.TooltipText = name;
            string artist = Metadata.GetArtist(MainController.playlist.songs[MainController.currentSong]);
            SongArtist.Text = artist;
            SongArtist.TooltipText = artist;
            SongCover.Texture = ConvertToGodot.getCover(MainController.playlist.songs[MainController.currentSong]);
        }
        else
        {
            SongName.Text = "No song playing";
            SongName.TooltipText = "";
            SongArtist.Text = "No artist";
            SongArtist.TooltipText = "";
        }

        SongCover.Texture = SongCover.Texture == null ? DefaultCover : SongCover.Texture;
        TotalTime.Text = SaveSystem.GetTimeFromSeconds(Metadata.GetTotalTime(MainController.playlist.songs[MainController.currentSong]));
        Progress.MaxValue = MainController.player.Stream.GetLength();
        Play.Icon = !MainController.playing ? PlayIcon : PauseIcon;
        DiscordPresense.Call("setdetails", SongName.Text);
        tim = 1;
    }

    public void setTime(bool value)
    {
        MainController.time = (float)Progress.Value;
        if (MainController.playing) MainController.player.Play(MainController.time);
        canSetTime = false;
        tim = 1;
    }

    public void submitmeta()
    {
        MainController.EditMeta(AttributeEditor.songname, AttributeEditor.artist, AttributeEditor.coverpath);
        onLoadSong();
        attributesBeingedited = false;
    }

    public override void _Process(double delta)
	{
        if(MainController.player.Stream != null) CurrentTime.Text = SaveSystem.GetTimeFromSeconds(MainController.time);

        if (!canSetTime)
        {
            Progress.Value = MainController.time;
            tim += (float)delta;
            if (tim >= 1)
            {
                DiscordPresense.Call("setstate", CurrentTime.Text, !MainController.playing);
                tim = 0;
            }
        }
        else if (!MainController.playing)
        {
            MainController.time = (float)Progress.Value;
            tim += (float)delta;
            if (tim >= 0.3f)
            {
                DiscordPresense.Call("setstate", CurrentTime.Text, !MainController.playing);
                tim = 0;
            }
        }

        MainController.player.VolumeDb = (float)(VolumeSlider.Value != -50 ? VolumeSlider.Value : -80);
    }
}
