using Godot;

public partial class PlaylistPlayer : Button {
	public override void _Ready() {
		ButtonUp += Play;
		Main.instance.OnPlayingChanged += UpdateTexture;
	}

	public void Play() {
		if(Main.playlist_index == Main.looked_at_playlist) {
			Main.instance.FlipPlayingState();
		}
		else {
            Main.instance.LoadPlaylist(Main.looked_at_playlist);
            Main.instance.SetTrack(0);
        }
	}

	public void UpdateTexture() {
		Icon = ApplicationManager.GetThemeIcon((Main.playlist_index == Main.looked_at_playlist) && Main.playing ? "pause_icon" : "play_icon");
	}
}
