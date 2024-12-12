using Godot;

public partial class Player : Node
{
    [Export] public Main MainController;
    [Export] public Button Play, Next, Back, Loop;
    [Export] public Texture2D PlayIcon, PauseIcon, LoopOn, LoopOff;
    [Export] public Slider Progress;
    [Export] public Label CurrentTime, TotalTime, SongName, SongArtist;
    [Export] public TextureRect SongCover;
    [Export] public Slider VolumeSlider;

    [Export] public Button EditAttributes;
    [Export] public AttributeEditor AttributeEditor;

    bool canSetTime, attributesBeingedited, spacepressed, discordworking = true;
    float tim = 0; // time for fixed rate updater thingy

    disc DiscordPresense;

    public override void _Ready()
	{
        Loop.ButtonDown += setLoop;
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

        DiscordPresense = new disc();

        try
        {
            DiscordPresense.init();
        }
        catch 
        { 
            discordworking = false;
        }
    }

    public void editAttributes()
    {
        if(!attributesBeingedited)
        {
            AttributeEditor.open(SongName.Text, SongArtist.Text);
            attributesBeingedited = true;

            if (MainController.playing) MainController.Play();
        }
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

        TotalTime.Text = SaveSystem.GetTimeFromSeconds(Metadata.GetTotalTime(MainController.playlist.songs[MainController.currentSong]));
        Progress.MaxValue = MainController.player.Stream.GetLength();
        DiscordPresense.setdetails(SongName.Text);
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
        if(Input.IsKeyPressed(Key.Space) && !spacepressed) 
        {
            MainController.Play();
            spacepressed = true;
        }
        else if(!Input.IsKeyPressed(Key.Space) && spacepressed) spacepressed = false;

        if(MainController.player.Stream != null) CurrentTime.Text = SaveSystem.GetTimeFromSeconds(MainController.time);

        if(discordworking)
        {
            if (!canSetTime) // To avoid stupid memory leak, put on a low fixed refresh rate
            {
                Progress.Value = MainController.time;
                tim += (float)delta;
                if (tim >= 1)
                {
                    DiscordPresense.setstate(CurrentTime.Text, !MainController.playing);
                    tim = 0;
                }
            }
            else if (!MainController.playing)
            {
                MainController.time = (float)Progress.Value;
                tim += (float)delta;
                if (tim >= 0.3f)
                {
                    DiscordPresense.setstate(CurrentTime.Text, !MainController.playing);
                    tim = 0;
                }
            }
        }

        MainController.player.VolumeDb = (float)(VolumeSlider.Value != -50 ? VolumeSlider.Value : -80);
    }
}
