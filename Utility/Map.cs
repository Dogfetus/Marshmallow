using Godot;

public partial class Map : Node2D
{
	private Player _player;
	private TextureRect _texture;
	private Spawner _spawner;
	// Called when the node enters the scene tree for the first time.

	public override void _Ready()
	{
		_spawner = GetNode<Spawner>("/root/Node/world/EnemySpawner");
		_player = GetNode<Player>("/root/Node/world/Player");
		_texture = GetNode<TextureRect>("deathzone/TextureRect");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
	}

	public void OnDeathzoneEntered(Node2D body){
		if (body is Enemy){
			Enemy tmp = (Enemy)body;
			GD.Print("entered");
			_spawner.Decrease();
			_player.AddScore(tmp.Worth());
			Texture2D texture = (Texture2D)GD.Load("res://Assets/marshmallow/shmore.png");
			_texture.Texture = texture;
			body.QueueFree();
		}
	}

	public void OnFirePlaceEntered(Node2D body){
		if (body is Enemy enemy){
			enemy.StartRoasting();
		}
	}
	public void OnFirePlaceExited(Node2D body){
		if (body is Enemy enemy){
			enemy.StopRoasting();
		}
	}
}
