using Godot;

public partial class Bubble : Label
{
	private Timer _timer;
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
		_timer = GetNode<Timer>("Timer");
		_timer.Timeout += () => clearBubble();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	private void clearBubble(){
		Text = "";
	}

	public void setTimer(float sec){     
		// Stop the timer if it's running
		if (_timer.IsStopped() == false)
		{
			_timer.Stop();
		}

		// Start the timer for 10 seconds
		_timer.Start(sec);
	}
}
