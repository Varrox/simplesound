using Godot;
using System;

public partial class ImportPlaylist : Button
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		ButtonDown += ImportFile;
	}

	public void ImportFile()
	{

	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}
}
