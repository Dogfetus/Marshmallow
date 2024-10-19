using Godot;
using System;

public partial class Player : CharacterBody2D
{
	public const float Speed = 120.0f;
	public const float BoostSpeed = 200.0f;
	public const float JumpVelocity = -400.0f;
	public float health = 100.0f;
	public float score = 0.0f;
	private Bubble _speechBubble;
	private AnimatedSprite2D _sprite;
	private TextEdit _text;
	private Label _score;
	private ProgressBar _healthBar;
	public string Message { get; set; } = "";


	// for correct sprite:
	public string spriteAction = "idle";
	public string spriteDirection = "up";

	// happens when player is created
	public override void _Ready(){
		_sprite = GetNode<AnimatedSprite2D>("Sprit");
		_text = GetNode<TextEdit>("CanvasLayer/TextEdit");
		_speechBubble = GetNode<Bubble>("Bubble");
		_score = GetNode<Label>("CanvasLayer/ScoreLabel");
		_healthBar = GetNode<ProgressBar>("HealthBar");
		_healthBar.Visible = false;
	}

	public override void _PhysicsProcess(double delta)
	{
		// GD.Print(score);
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
			UpdateScore();
		}
	}

	// to 
	private void _HandleText(){
		Message = _text.Text;
		_speechBubble.Text = Message;
		_speechBubble.setTimer((float)(4.0f + 0.025*Message.Length));
		_text.Text = "";
	}

	public void clearMessage(){
		Message = "";
	}

	public void TakeDamage(float damage){
		health -= damage;	
		GD.Print("Took damage, new health: ", health);
		_healthBar.Value = health;
		_healthBar.Visible = true;
		if (health <= 0){
			GetTree().ChangeSceneToFile("res://scenes/DeathScreen.tscn");
		}
	}

	public void AddScore(float dcore){
		score += (int)dcore;
	}

	public void UpdateScore(){
		_score.Text = "Score: " + score;
	}
}
