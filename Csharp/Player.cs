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

    public override void _Ready()
	{
        Loop.ButtonDown += setLoop;
        Play.ButtonDown += setPlay;
        Next.ButtonDown += () => move(1);
        Back.ButtonDown += () => move(-1);
        MainController.OnLoadSong += onLoadSong;
        Progress.DragEnded += setTime;
        Progress.DragEnded += (bool value) => canSetTime = true;
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
            SongName.Text = SaveSystem.GetName(MainController.playlist.songs[MainController.currentSong]);
            SongArtist.Text = Metadata.GetArtist(MainController.playlist.songs[MainController.currentSong]);
            SongCover.Texture = ConvertToGodot.getCover(MainController.playlist.songs[MainController.currentSong]);
        }
        else
        {
            SongName.Text = "No song playing";
            SongArtist.Text = "Artist";
        }

        SongCover.Texture = SongCover.Texture == null ? DefaultCover : SongCover.Texture;
        TotalTime.Text = SaveSystem.GetTimeFromSeconds((float)MainController.player.Stream.GetLength());
        Progress.MaxValue = MainController.player.Stream.GetLength();
        Play.Icon = !MainController.playing ? PlayIcon : PauseIcon;
        DiscordPresense.Call("setdetails", SongName.Text);
    }

    public void setTime(bool value)
    {
        MainController.time = (float)Progress.Value;
        if (MainController.playing) MainController.player.Play(MainController.time);
        canSetTime = false;
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

        if (!canSetTime) Progress.Value = MainController.time;
        else if (!MainController.playing) MainController.time = (float)Progress.Value;


        MainController.player.VolumeDb = (float)(VolumeSlider.Value != -50 ? VolumeSlider.Value : -80);
        DiscordPresense.Call("setstate", CurrentTime.Text, !MainController.playing);
    }
}
