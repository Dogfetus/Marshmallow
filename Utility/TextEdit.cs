using Godot;
using System;

public partial class TextEdit : Godot.TextEdit
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = false;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		if (Input.IsActionJustPressed("write")){
			// Visible = false;
		}
	}
}