using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 120.0f;
	public const float BoostSpeed = 200.0f;
	public const float JumpVelocity = -400.0f;
	private AnimatedSprite2D _sprite;

	// for correct sprite:
	public string spriteAction = "idle";
	public string spriteDirection = "up";

	// happens when player is created
	public override void _Ready(){
		_sprite = GetNode<AnimatedSprite2D>("Sprit");
	}

	public override void _PhysicsProcess(double delta)
	{

		
		// simple movement
		float speed = Input.IsActionPressed("sprint") ? BoostSpeed : Speed;
		Vector2 direction = Input.GetVector("left", "right", "up", "down");
		if (direction != Vector2.Zero)
		{
			spriteDirection = Math.Abs(direction.X) > Math.Abs(direction.Y) ? "side" : (direction.Y > 0 ? "down" : "up");
			_sprite.FlipH = direction.X > 0;
			spriteAction = "walk";
			Velocity = direction * speed;
		}
		else
		{
			spriteAction = "idle";
			Velocity = Vector2.Zero;
		}

		_sprite.Animation = spriteAction + "_" + spriteDirection;
		MoveAndSlide();
	}
}
