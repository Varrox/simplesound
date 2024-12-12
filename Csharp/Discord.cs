using
public class disc
{
    DiscordRpcClient client;
    RichPresence presence;
    public void init()
    {
        client = new DiscordRpcClient("1313226375007703160");

        client.Initialize();

        presence = new RichPresence();
    }

    public void setdetails(string song)
    {
        presence.Details = "Listening to " + song;
        client.SetPresence(presence);
    }

    public void setstate(string time, bool paused)
    {
        presence.State = time + (paused ? " (Paused)" : "");
    }
}
