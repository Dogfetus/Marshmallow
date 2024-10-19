using Godot;
using System;

public partial class MessageDelay : Timer
{

	public void Set(float sec){     
		// Stop the timer if it's running
		if (IsStopped() == false)
		{
			Stop();
		}
		// Start the timer for 10 seconds
		Start(sec);
	}

	public void Restart(){     
		// Stop the timer if it's running
		if (IsStopped() == false)
		{
			Stop();
		}
		// Start the timer for 10 seconds
		Start();
	}
}
