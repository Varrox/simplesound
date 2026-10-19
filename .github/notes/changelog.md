# simplesound alpha-v1.3 changelog

#### **8 months, and 200 commits total!**

## General Improvements

- Settings are stored more organized now (so your blur quality and vsync settings will be reset, sorry).
- Main window size is saved on close and loaded back up on open
- All instances of audio files being referred to as music or songs have been removed and now the more general (and less in the legal gray zone for the downloader) term 'track' is used instead.
- Backwards compatibility for settings / playlist variable names that are renamed after this version and forward.

## Features

- Track Downloading with yt-dlp for fast migration to simplesound from other platforms.
- EQ and Reverb audio settings.
- System tray item added for simplesound.
- Copy system info button added to settings menu.

## UI Improvements

- The current playing track will have a small audio visualizer in place of its number.
- The font of the current playing track's name and the playlist it's from, are now green to differentiate them from non playing tracks and playlists.
- Streamer button font is red when the streamer window is open.
- Settings menu tabs UI improved (tool tips, selected tab indicator, etc).
- All icons are now SVGs for better visibility.
- Improved selected song / playlist button highlight visibility.
- Playlist scroll positions are saved (only runtime), so no more scrolling back to the position you were at before.

## Optimizations

- Using ATL now over Taglib for faster metadata loading performance.
- Instant playlist loading (with the cost of no more separation between tracks).
- Exports from now on will be using a custom export template of godot that strips away all unnecessary parts of the engine like 3D, 2D, and physics. This decreases the size of simplesound by 15mb - 25mb.
- Max FPS setting and optional (on by default) setting that reduces max FPS on window focus lost
- Faster playlist / savedata json file loading.

# Breaks Compatibility

- Savedata has been completely redone, so any settings you had on 1.2 will be reset unfortunately. Though there wasn't many settings anyways (only blur settings and vsync) so its fine unless your system can't handle the blur.

# Notes

- I would like to mention that the downloader is not meant for piracy, but rather for a quick migration from mainstream platforms to this software. It would be more suitable for someone to buy audio tracks from their favorite artists, as those bought ones will have better quality than downloaded tracks. Please support artists.
- The plan to maintain the goal set above is to use simplesound as a way to psyop users into becoming audiophiles, so they HAVE to get the highest quality tracks, which typically are paid for.