using Godot;
using System;

public partial class Spawner : Node2D
{
	// the enemies
    // Reference to the enemy scene, loaded from a separate .tscn file
    private PackedScene enemyScene = GD.Load<PackedScene>("res://Entities/enemy.tscn");

	// then we also need randomness to find their spawnpoint
    private RandomNumberGenerator rng = new RandomNumberGenerator();

	// max enemies:
	private int _currentEnemies = 0;
	private int _maxEnemies = 10;
	
	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
	    var timer = GetNode<Timer>("Timer");
		timer.Timeout += () => SpawnEnemy();
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	public void SpawnEnemy()
	{

		// if 10 enemies are spawned just return
		if (_currentEnemies >= _maxEnemies)
		{
			return;	
		}

		if (enemyScene == null)
        {
            GD.PrintErr("Enemy scene not loaded properly.");
            return;
        }

        // Instance a new enemy
        Enemy enemyInstance = (Enemy)enemyScene.Instantiate();

        // Set a random position within the screen boundaries (or your preferred spawn area)
        float randomX = rng.RandfRange(500, 1500);
        float randomY = rng.RandfRange(250, 1250);
        enemyInstance.Position = new Vector2(randomY, randomX);

        // Add the enemy to the scene tree
        AddChild(enemyInstance);

        GD.Print("Spawned enemy at position: ", enemyInstance.Position, ", nr of enemies: ", _currentEnemies);

		_currentEnemies++;
	}
}
