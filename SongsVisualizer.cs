using Godot;
using System;

public partial class SongsVisualizer : ScrollContainer
{
	[Export] public PackedScene Template;
    [Export] public Main main;
	[Export] public PlaylistsVisualizer PlaylistsVisualizer;

	public override void _Ready()
	{

	}

	public override void _Process(double delta)
	{

	}
}
