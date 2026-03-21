using Godot;

public struct SongData
{
    public string title, artist, time;
    public bool explicit_lyrics, corrupt;

    public SongData(string file)
    {
        title = Tools.GetMediaTitle(file);
        artist = Metadata.GetArtist(file);
        time = Tools.SecondsToTimestamp(Metadata.GetTotalTime(file));
        explicit_lyrics = Metadata.IsExplicit(file);
        corrupt = Metadata.IsFileCorrupt(file);
    }
}