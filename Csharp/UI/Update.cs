using Godot;
using Godot.Collections;
using System.Collections.Generic;
using System.IO;

public partial class Update : Button
{
	HttpRequest http_client;

    [Export] Texture2D updatable, non_updatable;

    public bool updating;
	public bool can_be_updated;

    public string download_folder;

    CurrentRequest request;

    enum CurrentRequest
    {
        Version,
        Download,
        Downloading
    }

	public override void _Ready()
	{
        if (OS.HasFeature("editor"))
        {
            can_be_updated = false;
            return;
        }

        download_folder = new FileInfo(OS.GetExecutablePath()).Directory.FullName;

        http_client = new HttpRequest();
        AddChild(http_client);
        http_client.RequestCompleted += HttpClientRequestCompleted;

        request = CurrentRequest.Version;
        http_client.Request("https://api.github.com/repos/realbucketofchicken/Simplaudio/releases/latest");

        ButtonUp += StartUpdate;
	}

	public void StartUpdate()
	{
        if (!can_be_updated || updating) return;

        request = CurrentRequest.Download;
        http_client.Request("https://api.github.com/repos/godotengine/godot/releases/latest");
    }

    private void HttpClientRequestCompleted(long result, long responseCode, string[] headers, byte[] body)
    {
        if (result != (long)HttpRequest.Result.Success)
        {
            Debug.ErrorLog($"Https request failed. Something went wrong server side: {result}");
            return;
        }

        Dictionary json = (Dictionary)Json.ParseString(body.GetStringFromUtf8());


        switch(request)
        {
            case CurrentRequest.Version:
                can_be_updated = (string)json["name"] != (string)ProjectSettings.GetSetting("application/config/version");
                TooltipText = $"Newest version: {json["name"]}";
                Icon = can_be_updated ? updatable : non_updatable;
                break;
            case CurrentRequest.Download:
                Download(json);
                break;
            case CurrentRequest.Downloading:
                Debug.Log("Downloaded! Now installing");
                Install(http_client.DownloadFile);
                http_client.DownloadFile = string.Empty; // Clear
                updating = false;
                break;
        }
    }

    public void Install(string file)
    {
        // Delete All previous files

        string[] download_folder_files = Directory.GetFiles(download_folder);

        for(int i = 0; i < download_folder_files.Length; i++)
        {
            if (download_folder_files[i] != file)
            {
                if (!download_folder_files[i].EndsWith('/'))
                    File.Delete(download_folder_files[i]);
                else
                    Directory.Delete(download_folder_files[i]);
            }
        }

        // Install new files

        ZipReader zip_reader = new ZipReader();
        Error error = zip_reader.Open(file);

        if (error != Error.Ok)
        {
            Debug.ErrorLog("Install Failed. Zip is corrupted");
            return;
        }

        List<string> files = new List<string>(zip_reader.GetFiles());

        for (int i = 0; i < files.Count; i++) 
        {
            string path = Path.Combine(download_folder, files[i].Substring(files[i].IndexOf("/") + 1));
            if (!files[i].EndsWith('/'))
            {
                Debug.Log(path);
                File.WriteAllBytes(path, zip_reader.ReadFile(files[i]));
            }
            else
            {
                Directory.CreateDirectory(path);
            }
        }

        zip_reader.Close();
        
        File.Delete(file);

        // Restart

        ConfirmationWindow confirmation = (ConfirmationWindow)Globals.confirmation_window.Instantiate();

        confirmation.Message = "Restart simplesound?";
        confirmation.AcceptText = "Yes";
        confirmation.DeclineText = "No";
        confirmation.CancelText = "";

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

    public void Download(Dictionary json)
    {
        Debug.Log("Downloading");

        Array<Dictionary> assets = (Array<Dictionary>)json["assets"];

        int download_index = -1;

        for(int i = 0; i < assets.Count; i++)
        {
            string name = ((string)assets[i]["name"]).ToLower();

            if (name == "Godot_v4.4.1-stable_mono_win64.zip".ToLower())//(name.Contains(OS.GetName().ToLower()))
            {
                download_index = i;
            }
        }

        if (download_index == -1)
        {
            Debug.ErrorLog("Current platform not found in available downloads");
            return;
        }

        request = CurrentRequest.Downloading;
        updating = true;

        http_client.DownloadFile = $"{download_folder}\\Download.zip";
        http_client.Request((string)assets[download_index]["browser_download_url"]);
    }
}
