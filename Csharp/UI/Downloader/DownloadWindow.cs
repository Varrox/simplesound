using Godot;
using System;
using System.IO;

public partial class DownloadWindow : EditorWindow
{
    public void Open()
    {
        Globals.file_dialog.Reparent(this);
        Show();
    }

    public void Close()
    {
        Globals.file_dialog.Reparent(Globals.self);
        Hide();
        Globals.player.interrupted = false; 
    }

    public void RunDownloadCommand(string url)
    {
        OS.Execute(Globals.save_data.application_settings.ytdlp_location, new[]{"-x", "--mp3", "-o", $"'{Path.Combine(SaveSystem.USER_DATA, "Downloaded Music")}'", url});
    }
}
