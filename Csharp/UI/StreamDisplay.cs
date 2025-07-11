using Godot;
using System;

public partial class StreamDisplay : Window
{
	[Export] public TextureRect cover_art, background;
	[Export] public Label song, artist;
}
