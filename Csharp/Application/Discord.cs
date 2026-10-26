using Godot;
using DiscordRPC;
using DiscordRPC.Message;

public partial class Discord : Node {
    private const string DISCORD_APP_ID = "1313226375007703160";

    private static DiscordRpcClient _client;
    private static bool _set_up;

    private static HttpRequest _request;
    private static string _link, _cover_link;

    private const string LOGO = "logotrans", LOGO_PRIDE_MONTH = "logogay", LOGO_JOLLY = "logochrimis";

    public override void _Ready() {
        // Create the client and setup some basic events
        _client = new DiscordRpcClient(DISCORD_APP_ID);

        // Connect to the RPC
        _client.Initialize();

        _client.OnReady += _FinishSetUp;

        if (_request != null) _request.CancelRequest();
        else {
            _request = new HttpRequest();
            AddChild(_request);

            _request.RequestCompleted += _RequestFinished;
        }
    }

    private void _FinishSetUp(object sender, ReadyMessage msg) {
        _set_up = true;
        UpdateTrack();
    }

    public static void ShutDown() { // Cleanup
        _client.Dispose();
    }

    public static void UpdateTrack() {
        if (!_set_up) return;
        _Check();
    }

    private static void _Check() {
        string input_link = Metadata.GetShareLink(Main.GetCurrentTrack());

        if (string.IsNullOrEmpty(input_link)) {
            _cover_link = "";
            _FinishUpdateTrack();
            return;
        }

        _link = ParseURL(input_link);

        if (_link != input_link) {
            _cover_link = _link;
            _FinishUpdateTrack();
            return;
        }

        _request.CallDeferredThreadGroup("cancel_request");
        _request.CallDeferredThreadGroup("request", new Variant[] { _link, new string[] { "Content-Type: application/json" } });
    }

    private static void _RequestFinished(long result, long responseCode, string[] headers, byte[] body) {
        if (result != (long)HttpRequest.Result.Success) {
            GD.PushError($"Https request failed. Something went wrong server side: {result}");
            return;
        }

        string text = body.GetStringFromUtf8();

        // Idc if it can break, it works

        if (_link.Contains("open.spotify.com") || _link.Contains("soundcloud.com")) {
            string meta = "<meta property=\"og:image\" content=";

            text = text.Substring(text.IndexOf(meta) + meta.Length + 1);
            text = text.Substring(0, text.IndexOf('"'));
        }
        else if (_link.Contains("music.apple.com")) {
            string meta = "<source sizes=\" (max-width:1319px) 296px,(min-width:1320px) and (max-width:1679px) 316px,316px\" srcset=\"";

            text = text.Substring(text.IndexOf(meta) + meta.Length);
            text = text.Substring(0, text.IndexOf("296w,https://"));
        }

        _link = text;

        _cover_link = _link;
        _FinishUpdateTrack();
    }

    private static void _FinishUpdateTrack() {
        if (!_set_up) return;

        bool cover = string.IsNullOrEmpty(_cover_link) || _cover_link.Length > 255;

        TrackData track_data = new TrackData(Main.GetCurrentTrack());

        RichPresence presence = new RichPresence() {
            Details = track_data.title,
            State = track_data.artist,
            Assets = new Assets() {
                LargeImageKey = cover ? GetLogo() : _cover_link,
                SmallImageKey = cover ? "" : GetLogo(),
                LargeImageText = "simplesound is an open source audio player",
                LargeImageUrl = "https://github.com/Varrox/simplesound"
            },
            Type = ActivityType.Listening,
            Buttons = [ new DiscordRPC.Button() { Label = "Open track", Url = Metadata.GetShareLink(Main.GetCurrentTrack()) } ]
        };

        _client.SetPresence(presence);
    }

    public static string GetLogo() {
        int month = (int)Time.GetDateDictFromSystem()["month"];

        if(month == 6) return LOGO_PRIDE_MONTH;
        else if(month == 12) return LOGO_JOLLY;
        else return LOGO;
    }

    public static string ParseURL(string url) {
        string converted_url = url;
        if (url.Contains("www.youtube") || url.Contains("https://youtu.be/")) {
            converted_url = "https://i.ytimg.com/vi/";

            string[] split = url.Substring(("https://").Length - 1).Split("/");
            string video_id = split[split.Length - 1];
            converted_url += video_id.Length > 0 ? video_id.Substring("watch?v=".Length).Split("?=")[0] : "";
            converted_url += "/hqdefault.jpg";
        }

        return converted_url;
    }
}
