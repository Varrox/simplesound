# simplesound alpha-v1.3 changelog

#### **5 months, and 145 commits total!**

## General Improvements

- Settings are stored more organized now (so your blur quality and vsync settings will be reset, sorry).
- Main window size is saved on close and loaded back up on open

## Features

- Song Downloading with yt-dlp.
- EQ and Reverb audio settings.
- System tray item added for simplesound.
- Copy system info button added to settings menu.

## UI Improvements

- The current playing song will have a small audio visualizer in place of its number.
- The font of the current playing song's name and the playlist it's from, are now green to differentiate them from non playing songs and playlists.
- Streamer button font is red when the streamer window is open.
- Settings menu tabs UI improved (tool tips, selected tab indicator, etc).
- Folder Icon improved for better visibility, and consistency with other icons.

## Optimizations

- Instant playlist loading (with the cost of no more separation between songs).
- Exports from now on will be using a custom export template of godot that strips away all unnecessary parts of the engine like 3D, or physics. This decreases the size of simplesound by 15mb - 25mb.
- Max FPS setting and optional (on by default) setting that reduces max FPS on window focus lost

# Breaks Compatibility

Savedata has been redone, so any settings you had on 1.2 will be reset unfortunately. Though there wasn't many settings anyways (only blur settings and vsync) so its fine unless your system can't handle the blur.