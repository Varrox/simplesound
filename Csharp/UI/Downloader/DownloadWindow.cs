using Godot;
using System;
using System.Collections.Generic;
using System.IO;

public partial class DownloadWindow : EditorWindow
{
    [Export] public Button no_playlist;
    [Export] public VBoxContainer playlist_container, link_container;
    [Export] public PackedScene playlist_display;
    [Export] public LineEdit link;
    [Export] public Button add_link, download, close;

    public Action<int> OnSelectPlaylist;

    public int selected_playlist = -1;

    public List<string> links = new List<string>();

    public override void _Ready()
    {
        base._Ready();
        download.ButtonUp += Download;
        close.ButtonUp += Close;
        no_playlist.ButtonUp += () => {OnSelectPlaylist?.Invoke(-1); no_playlist.SelfModulate = Globals.lower_highlight;};
        OnSelectPlaylist += ClearNoneSelected;
        add_link.ButtonUp += AddLink;
    }

    public void AddLink()
    {
        links.Add(link.Text);
        HBoxContainer container = new HBoxContainer();
        link_container.AddChild(container);

        Label label = new Label();
        label.Text = link.Text;

        container.AddChild(label);

        label.SizeFlagsHorizontal = Control.SizeFlags.ExpandFill;
        label.ClipText = true;
        label.TextOverrunBehavior = TextServer.OverrunBehavior.TrimEllipsis;

        Button button = new Button();
        button.Text = "X";

        container.AddChild(button);

        button.ButtonUp += () => {links.RemoveAt(container.GetIndex()); container.QueueFree();};

        link.Text = "";
    }

    public void ClearNoneSelected(int index)
    {
        selected_playlist = index;
        if (index != -1) no_playlist.SelfModulate = new Color(1, 1, 1, 1);
    }

    public void Open()
    {
        Globals.file_dialog.Reparent(this);

        no_playlist.SelfModulate = Globals.lower_highlight;
        OnSelectPlaylist?.Invoke(-1);

        for(int i = 0; i < Globals.main.playlists.Count; i++)
        {
            DownloadPlaylistDisplay display = playlist_display.Instantiate() as DownloadPlaylistDisplay;
            playlist_container.AddChild(display);
            display.Init(Globals.main.playlists[i], i);
        }

        Show();
    }

    public void Close()
    {
        if(Globals.self == null)
        {
            GetTree().Quit();
            return;
        }

        Globals.file_dialog.Reparent(Globals.self);
        Hide();
        Clear();
        Globals.player.interrupted = false; 
    }

    public void Clear()
    {
        link.Text = "";

        foreach(Node node in link_container.GetChildren())
        {
            node.QueueFree();
        }

        links.Clear();

        foreach(Node node in playlist_container.GetChildren())
        {
            if(node != no_playlist)
            {
                if(node is DownloadPlaylistDisplay)
                    OnSelectPlaylist -= (node as DownloadPlaylistDisplay).ClearSelected;
                node.QueueFree();
            }
        }
    }

    public void Download()
    {
        foreach(string url in links)
        {
            string[] songs = RunDownloadCommand(url, selected_playlist != -1 ? Globals.main.playlists[selected_playlist].name : "");

            if(songs == null || songs.Length == 0)
                continue;
            
            if(selected_playlist != -1)
            {
                GD.Print(songs);
                Globals.main.playlists[selected_playlist].songs.AddRange(songs);
                Globals.main.playlists[selected_playlist].Save();
                Globals.main.Refresh();
            }
            else // Open in file explorer
            {
                OS.ShellShowInFileManager(songs[0], true);
            }
        }
    }

    public string[] RunDownloadCommand(string url, string folder)
    {
        if(Globals.self == null) // If globals doesn't exist
            return null;
        
        var output = new Godot.Collections.Array();

        string directory = folder == "" ? Path.Combine(SaveSystem.USER_DATA, "Music") : Path.Combine(SaveSystem.USER_DATA, "Music", folder);

        if(!Directory.Exists(directory))
            Directory.CreateDirectory(directory);
        
        const string file = "%(title)s.%(ext)s";

        string output_directory = Path.Combine(directory, file);

        string selected_format = Constants.download_formats[Globals.save_data.application_settings.download_format];

        List<string> arguments = new List<string>(new[]{
            "-x", 
            "--audio-format", selected_format, 
            "--add-metadata", 
            "--quiet",
            "--no-warnings",
            "--no-progress",
            "--print", "after_move:filepath", 
            "--print", "Link %(webpage_url)s",
            "-o", 
            output_directory, 
            url
        });

        if(selected_format == "mp3")
            arguments.Add("--embed-thumbnail");
        
        if(url.Contains("list="))
            arguments.Insert(4,"--yes-playlist");

        int result = OS.Execute(Globals.save_data.application_settings.ytdlp_location, arguments.ToArray(), output, true, false);

        List<string> songs = new List<string>();

        foreach(string song in output)
        {
            if (!string.IsNullOrEmpty(song))
            {
                if (song.StartsWith("Link"))
                {
                    string[] split = song.Trim().Split("\n");
                    for(int i = 0; i < split.Length; i += 2)
                    {
                        Metadata.WriteToComment(split[i + 1], split[i]);
                        songs.Add(split[i + 1]);
                    }
                }
            }
                
        }

        return songs.ToArray();
    }
}
