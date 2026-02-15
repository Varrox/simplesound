using Godot;
using System;

public partial class DownloadPlaylistDisplay : Button
{
	[Export] public TextureRect cover;
    [Export] public Label playlist_name, songs;

	int playlist_index;

	public override void _Ready()
	{
		ButtonUp += Set;
	}

	public void Set()
    {
        Globals.download_window.OnSelectPlaylist?.Invoke(playlist_index);
        Globals.download_window.selected_playlist = playlist_index;
        SelfModulate = Globals.lower_highlight;
    }

	public void ClearSelected(int index)
    {
        if (index != playlist_index) SelfModulate = new Color(1, 1, 1, 1);
    }

	public void Init(Playlist playlist, int index)
    {
        cover.Texture = ConvertToGodot.LoadImage(playlist.cover) ?? Globals.default_cover;
        playlist_name.Text = playlist.name;

        string amount = $"{playlist.songs.Count}{(playlist.songs.Count != 1 ? " songs" : " song")}";

        if (playlist.type != Playlist.PlaylistType.Album)
        {
            if (playlist.songs == null)
                songs.Text = "0 songs";
            else
                songs.Text = $"{amount}{(playlist.artist != null ? $" {Constants.dot} {playlist.artist}" : "")}";
        }
        else
        {
            songs.Text = $"Album  {Constants.dot}  {playlist.artist ?? (playlist.songs.Count.ToString() + (playlist.songs.Count != 1 ? " songs" : " song"))}";
        }

        playlist_index = index;

        TooltipText = $"{playlist.name} - {amount}";

        Globals.download_window.OnSelectPlaylist += ClearSelected;
    }
}
