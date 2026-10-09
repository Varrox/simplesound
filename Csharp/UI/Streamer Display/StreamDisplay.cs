using Godot;
using System;

public partial class StreamDisplay : Window
{
	[Export] public TextureRect cover_texture_rect, background_texture_rect;
	[Export] public Label track_name_label, artist_label;
}
