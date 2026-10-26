using Godot;

public partial class PlaylistPlayer : Button {
	public override void _Ready() {
		ButtonUp += Play;
		Globals.main.OnPlayingChanged += UpdateTexture;
	}

	public void Play() {
		if(Globals.main.playlist_index == Globals.main.looked_at_playlist) {
			Globals.main.FlipPlayingState();
		}
		else {
            Globals.main.LoadPlaylist(Globals.main.looked_at_playlist);
            Globals.main.SetTrack(0);
        }
	}

	public void UpdateTexture() {
		Icon = ApplicationManager.GetThemeIcon((Globals.main.playlist_index == Globals.main.looked_at_playlist) && Globals.main.playing ? "pause_icon" : "play_icon");
	}
}
