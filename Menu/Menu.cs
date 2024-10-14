using Godot;
using System;

public partial class Menu : Control
{
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		Visible = true;
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	// function which is called when start button is pressed
	private void OnStartButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/game.tscn");
	}

	// function which is called when option button is pressed
	private void OnOptionButtonPressed()
	{
		GetTree().ChangeSceneToFile("res://scenes/game.tscn");
	}

	// function which is called when exit button is pressed
	private void OnExitButtonPressed()
	{
		GetTree().Quit();
	}

}
