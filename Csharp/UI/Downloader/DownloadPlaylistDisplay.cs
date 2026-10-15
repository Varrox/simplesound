using Godot;

public partial class DownloadPlaylistDisplay : Button
{
	[Export] public TextureRect cover_texture_rect;
    [Export] public Label playlist_name_label, track_count_label;

	int playlist_index;

	public override void _Ready() {
		ButtonUp += Set;
	}

	public void Set()
    {
        Globals.download_window.EmitSignal("OnSelectPlaylist", playlist_index);
        Globals.download_window.selected_playlist = playlist_index;

        ApplicationManager.SetRadioSelected(this, true);
    }

	public void ClearSelected(int index)
    {
        if (index != playlist_index) ApplicationManager.SetRadioSelected(this, false);
    }

	public void Init(Playlist playlist, int index)
    {
        cover_texture_rect.Texture = ConvertToGodot.LoadImageFromFile(playlist.cover) ?? ApplicationManager.theme.GetIcon("default_cover_icon", Constants.THEME_TYPE);
        playlist_name_label.Text = playlist.name;

        string amount = $"{playlist.tracks.Count}{(playlist.tracks.Count != 1 ? " tracks" : " track")}";

        if (playlist.type != Playlist.PlaylistType.Album)
        {
            if (playlist.tracks == null)
                track_count_label.Text = "0 tracks";
            else
                track_count_label.Text = $"{amount}{(playlist.artist != null ? $" {Constants.DOT} {playlist.artist}" : "")}";
        }
        else
        {
            track_count_label.Text = $"Album  {Constants.DOT}  {playlist.artist ?? (playlist.tracks.Count.ToString() + (playlist.tracks.Count != 1 ? " tracks" : " track"))}";
        }

        playlist_index = index;

        TooltipText = $"{playlist.name} - {amount}";

        Globals.download_window.OnSelectPlaylist += ClearSelected;
    }
}
