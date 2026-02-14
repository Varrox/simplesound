using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.IO;

public partial class Update : Button
{
    readonly string download_folder = new FileInfo(OS.GetExecutablePath()).Directory.FullName;
    readonly string download_file = $"{new FileInfo(OS.GetExecutablePath()).Directory.FullName}\\Download.zip";
    const string repo_link = "https://api.github.com/repos/Varrox/simplesound/releases/latest";

	HttpRequest http_client;
    bool updating, can_be_updated;
    CurrentRequest current_request;

    enum CurrentRequest
    {
        Version,
        Download,
        Install
    }

	public override void _Ready()
	{
        if (OS.HasFeature("editor")) // If editor, do not allow for updating at all.
            QueueFree();

        http_client = new HttpRequest();
        AddChild(http_client);

        ButtonUp += StartUpdate;

        http_client.RequestCompleted += RequestCompleted;

        CheckUpdate();
	}

	public void StartUpdate()
	{
        if (!can_be_updated || updating) return;

        current_request = CurrentRequest.Download;
        http_client.Request(repo_link);
    }

    public void CheckUpdate()
    {
        current_request = CurrentRequest.Version;
        http_client.Request(repo_link);
    }

    private void RequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        if (result != (long)HttpRequest.Result.Success)
        {
            GD.PushError($"Https request failed. Something went wrong server side: {result}");
            return;
        }

        Dictionary json = (Dictionary)Json.ParseString(body.GetStringFromUtf8());

        switch(current_request)
        {
            case CurrentRequest.Version: // DONE ONCE: To check if there is update available.
                can_be_updated = (string)json["name"] != (string)ProjectSettings.GetSetting("application/config/version");
                TooltipText = $"Update Available! {json["name"]}";
                Visible = can_be_updated;
                break;
            case CurrentRequest.Download: // Start downloading zip file.
                StartDownload(json);
                break;
            case CurrentRequest.Install: // Install new version
                GD.Print("Downloaded! Now installing");
                Install(download_file);
                http_client.DownloadFile = string.Empty; // Clear.
                updating = false;
                GD.Print("Update Fully Installed!"); // Finished Downloading.
                break;
        }
    }

    public void Install(string file)
    {
        // Read new files.

        ZipReader zip_reader = new ZipReader();
        Error error = zip_reader.Open(file);

        if (error != Error.Ok) // Cancel if ZIP is corrupted.
        {
            GD.PushError("Install Failed. Zip is corrupted");
            return;
        }

        // Delete All previous files.

        string[] download_folder_files = Directory.GetFiles(download_folder);

        for(int i = 0; i < download_folder_files.Length; i++)
        {
            if (download_folder_files[i] == file) // Skip new version.
                continue;
            
            if (Directory.Exists(download_folder_files[i])) // Delete Directory.
                Directory.Delete(download_folder_files[i]);
            else if (File.Exists(download_folder_files[i])) // Delete File.
                File.Delete(download_folder_files[i]);
        }

        // Install new files.

        string[] files = zip_reader.GetFiles();

        for (int i = 0; i < files.Length; i++)
        {
            string path = Path.Combine(download_folder, files[i].Substring(files[i].IndexOf("/") + 1));
            if (!files[i].EndsWith('/')) {
                File.WriteAllBytes(path, zip_reader.ReadFile(files[i]));
            }
            else {
                Directory.CreateDirectory(path);
            }
        }

        zip_reader.Close();
        
        File.Delete(file); // Delete .zip file.

        // Restart.

        ConfirmationWindow confirmation = (ConfirmationWindow)Globals.confirmation_window.Instantiate();

        confirmation.message = "Restart simplesound?";
        confirmation.accept_text = "Yes";
        confirmation.decline_text = "No";
        confirmation.cancel_text = "";

        confirmation.front = true;
        confirmation.free_on_close = true;

        GetTree().CurrentScene.AddChild(confirmation);

        confirmation.OnClose += Restart;
    }

    public void Restart(Confirm confirm)
    {
        if(confirm == Confirm.Accepted)
        {
            OS.ShellOpen(OS.GetExecutablePath());
            GetTree().Quit();
        }
    }

    public void StartDownload(Dictionary json)
    {
        GD.Print("Downloading");

        Array<Dictionary> assets = (Array<Dictionary>)json["assets"];

        int download_index = -1;

        for(int i = 0; i < assets.Count; i++) // Find download link index.
        {
            string name = ((string)assets[i]["name"]).ToLower();

            if (name.Contains(OS.GetName().ToLower()))
            {
                download_index = i;
            }
        }

        if (download_index == -1) // If download link not found, quit.
        {
            GD.PushError("Current platform not found in available downloads");
            return;
        }

        current_request = CurrentRequest.Install;
        updating = true;

        http_client.DownloadFile = download_file;
        http_client.Request((string)assets[download_index]["browser_download_url"]); // Request build zip file.
    }
}
