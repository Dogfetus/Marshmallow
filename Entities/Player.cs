using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 120.0f;
	public const float BoostSpeed = 200.0f;
	public const float JumpVelocity = -400.0f;
	private Bubble _speechBubble;
	private AnimatedSprite2D _sprite;
	private TextEdit _text;
	
	public string Message { get; set; } = "";


	// for correct sprite:
	public string spriteAction = "idle";
	public string spriteDirection = "up";

	// happens when player is created
	public override void _Ready(){
		_sprite = GetNode<AnimatedSprite2D>("Sprit");
		_text = GetNode<TextEdit>("CanvasLayer/TextEdit");
		_speechBubble = GetNode<Bubble>("Bubble");
	}

	public override void _PhysicsProcess(double delta)
	{
		// to type messages
		if (Input.IsActionJustPressed("write")){
			_text.Visible = true;
			_text.GrabFocus();
		} 

		if (_text.Visible && Input.IsActionJustPressed("return")){
			_text.ReleaseFocus();
			_text.Visible = false;
			_HandleText();
		}
		
		if (!_text.Visible){

			// simple movement
			GD.Print("Player: ", Position);
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

	// to 
	private void _HandleText(){
		Message = _text.Text;
		_speechBubble.Text = Message;
		_speechBubble.setTimer((float)(4.0f + 0.025*Message.Length));
		_text.Text = "";
	}
}
