using Godot;
using System;

public partial class Enemy : CharacterBody2D
{
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	private Node2D _player;  
	private const float _interactionRange = 100.0f;
	private Label _interactLabel;

    public override void _Ready()
    {
        _interactLabel = GetNode<Label>("Label");
        _player = GetNode<CharacterBody2D>("/root/Node/Player");
        _interactLabel.Visible = false; 
    }

	public override void _PhysicsProcess(double delta)
	{
		Vector2 velocity = Velocity;

        // Check the distance between the player and the enemy
        if (_player != null)
        {
			// added new vector offsett since this shit got retarded (offset made it correct)
            float distanceToPlayer = Position.DistanceTo(_player.Position + new Vector2(-225,-45));
            if (distanceToPlayer <= _interactionRange)
            {
                ShowInteractUI(true);
				if (_player.messages)
            }
            else
            {
                // Hide "Press E to interact" UI
                ShowInteractUI(false);
            }
        }

		MoveAndCollide(velocity);
	}

	private void ShowInteractUI(bool visible)
    {
       if (_interactLabel != null)
        {
            // Show or hide the label
            _interactLabel.Visible = visible;

        }
    }
}
