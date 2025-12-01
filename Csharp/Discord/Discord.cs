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

        RichPresence presence = new RichPresence()
        {
            Details = Globals.player.SongName.Text,
            State = Globals.player.SongArtist.Text,
            Assets = new Assets()
            {
                LargeImageKey = string.IsNullOrEmpty(cover_link) ? "logotrans" : cover_link,
                LargeImageText = name == "varrox" ? $"btw I ({client.CurrentUser.DisplayName}) made this software" : "simplesound is an open source music player",
                LargeImageUrl = "https://github.com/Varrox/simplesound"
            },
            Type = ActivityType.Listening,
            Buttons = new[]{ new DiscordRPC.Button() { Label = "Open song", Url = "https://lachee.dev/" } },
            DetailsUrl = Globals.main.current_share_link
        };

        client.SetPresence(presence);
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
            cover_link = "logotrans";
            FinishUpdateSong();
            return;
        }

        link = ParseURL(input_link);

        if (link != input_link)
        {
            cover_link = "logotrans";
            FinishUpdateSong();
            return;
        }

        request.CancelRequest();
        request.Request(link, ["Content-Type: application/json"]);
    }

    void RequestFinished(long result, long responseCode, string[] headers, byte[] body)
    {
        if (result != (long)HttpRequest.Result.Success)
        {
            GD.PushError($"Https request failed. Something went wrong server side: {result}");
            return;
        }

        string text = body.GetStringFromUtf8();

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
            converted_url = "https://i.ytimg.com/vi_webp/";

            var split = url.Right(-("https://").Length).Split("/");
            string video_id = split[split.Length];
            converted_url += video_id.Substring("watch?v=".Length).Split("?")[0];
            converted_url += "/maxresdefault.webp";
        }

        return converted_url;
    }
}
