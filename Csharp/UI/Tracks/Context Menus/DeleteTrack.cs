using Godot;
using System;

public partial class DeleteTrack : Button
{
	public override void _Ready()
	{
		ButtonDown += Remove;
    }

    ConfirmationWindow confirmation_window;


    public void Remove()
	{
		if(Globals.save_data.application_settings.remove_track_warning)
		{
			confirmation_window = Globals.confirmation_window.Instantiate() as ConfirmationWindow;

            confirmation_window.message = $"Are you sure you want to delete this track \'{Metadata.GetName(Globals.main.playlists[Globals.main.looked_at_playlist].songs[TracksContextMenuOpener.selected_track])}\' from this playlist?";
            confirmation_window.accept_text = "Yes";
            confirmation_window.decline_text = "No";
            confirmation_window.cancel_text = "";
            confirmation_window.free_on_close = true;
            confirmation_window.show_ignore = true;

            GetTree().CurrentScene.AddChild(confirmation_window);

            confirmation_window.OnClose += FinishRemoval;
            return;
        }

        FinishRemoval(Confirm.Accepted);
	}

	public void FinishRemoval(Confirm confirm)
	{
        if (confirmation_window != null) 
        {
            if (confirmation_window.ignored)
            {
                Globals.save_data.application_settings.remove_track_warning = false;
                Globals.save_data.Save();
            }

            if (confirm != Confirm.Accepted)
                return;
        }

        if (Globals.main.looked_at_playlist == Globals.main.playlist_index)
        {
            if (Globals.main.track_index == TracksContextMenuOpener.selected_track)
            {
                Globals.main.track_index += (TracksContextMenuOpener.selected_track == 0 ? -1 : 1);
                Globals.main.PlayTrack(Globals.main.track);
            }

            if (Globals.main.track_index > TracksContextMenuOpener.selected_track)
                Globals.main.track_index -= 1;
        }

        Globals.main.playlists[Globals.main.looked_at_playlist].songs.RemoveAt(TracksContextMenuOpener.selected_track);
        Globals.main.playlists[Globals.main.looked_at_playlist].Save();
        Globals.main.tracks_visualizer.Update();
    }
}
