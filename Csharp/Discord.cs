using Godot;
using DiscordRPC;
using DiscordRPC.Message;

public partial class Discord : Node
{
    public const string DISCORD_APP_ID = "1313226375007703160";
    public static DiscordRpcClient client;
    public static bool set_up;

    public HttpRequest request;
    string link;

    public static string cover_link;

    const string logo = "logotrans", logo_gay = "logogay", logo_chrimis = "logochrimis";

    public override void _Ready()
    {
        // Create the client and setup some basic events
        client = new DiscordRpcClient(DISCORD_APP_ID);

        //Connect to the RPC
        client.Initialize();

        client.OnReady += FinishSetUp;

        if (request != null)
        {
            request.CancelRequest();
        }
        else
        {
            request = new HttpRequest();
            AddChild(request);
            request.RequestCompleted += RequestFinished;
        }
    }

    public void FinishSetUp(object sender, ReadyMessage msg)
    {
        set_up = true;
        UpdateSong();
    }

    public void UpdateSong()
    {
        if (!set_up) return;
        Check();
    }

    public void FinishUpdateSong()
    {
        if (!set_up) return;

        string name = client.CurrentUser.Username;

        bool cover = string.IsNullOrEmpty(cover_link) || cover_link.Length > 255;

        RichPresence presence = new RichPresence()
        {
            Details = Globals.player.song_name.Text,
            State = Globals.player.song_artist.Text,
            Assets = new Assets()
            {
                LargeImageKey = cover ? GetLogo() : cover_link,
                SmallImageKey = cover ? "" : GetLogo(),
                LargeImageText = name == "varrox" ? $"btw I ({client.CurrentUser.DisplayName}) made this software" : "simplesound is an open source music player",
                LargeImageUrl = "https://github.com/Varrox/simplesound"
            },
            Type = ActivityType.Listening,
            Buttons = new[]{ new DiscordRPC.Button() { Label = "Open song", Url = Globals.main.current_share_link } }
        };

        client.SetPresence(presence);
    }

    public string GetLogo(){
        int month = (int)Time.GetDateDictFromSystem()["month"];
        if(month == 6)
            return logo_gay;
        else if(month == 12)
            return logo_chrimis;
        else
            return logo;
    }

    public static void ShutDown()
    {
        // Cleanup
        client.Dispose();
    }

    public void Check()
    {
        string input_link = Globals.main.current_share_link;

        if (string.IsNullOrEmpty(input_link))
        {
            cover_link = "";
            FinishUpdateSong();
            return;
        }

        link = ParseURL(input_link);

        if (link != input_link)
        {
            cover_link = link;
            FinishUpdateSong();
            return;
        }

        request.CallDeferredThreadGroup("cancel_request");
        request.CallDeferredThreadGroup("request", new Variant[] { link, new string[] { "Content-Type: application/json" } });
    }

    void RequestFinished(long result, long responseCode, string[] headers, byte[] body)
    {
        if (result != (long)HttpRequest.Result.Success)
        {
            GD.PushError($"Https request failed. Something went wrong server side: {result}");
            return;
        }

        string text = body.GetStringFromUtf8();

        // Idc if it can break, it works

        if (link.Contains("open.spotify.com") || link.Contains("soundcloud.com"))
        {
            string meta = "<meta property=\"og:image\" content=";

            text = text.Substring(text.IndexOf(meta) + meta.Length + 1);
            text = text.Substring(0, text.IndexOf('"'));
        }
        else if (link.Contains("music.apple.com"))
        {
            string meta = "<source sizes=\" (max-width:1319px) 296px,(min-width:1320px) and (max-width:1679px) 316px,316px\" srcset=\"";

            text = text.Substring(text.IndexOf(meta) + meta.Length);
            text = text.Substring(0, text.IndexOf("296w,https://"));
        }

        link = text;

        cover_link = link;
        FinishUpdateSong();
    }

    string ParseURL(string url)
    {
        string converted_url = url;
        if (url.Contains("www.youtube") || url.Contains("https://youtu.be/"))
        {
            converted_url = "https://i.ytimg.com/vi/";

            string[] split = url.Substring(("https://").Length - 1).Split("/");
            string video_id = split[split.Length - 1];
            converted_url += video_id.Length > 0 ? video_id.Substring("watch?v=".Length).Split("?=")[0] : "";
            converted_url += "/hqdefault.jpg";
        }

        return converted_url;
    }
}
