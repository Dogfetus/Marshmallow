using Godot;
using GroqApiLibrary;
using DotNetEnv;
using System.Text.Json.Nodes;
using Newtonsoft.Json.Linq;

public partial class Enemy : CharacterBody2D
{
    
    private GroqApiClient _ai;
	public const float Speed = 300.0f;
	public const float JumpVelocity = -400.0f;
	private Player _player;  
	private const float _interactionRange = 100.0f;
	private Label _interactLabel;
    // private bool _rating;
    // private bool _responding;
    private bool _spokenTo;

    private float health = 100.0f;

    public override async void _Ready()
    {
        // load env variables:
        Env.Load();
        string apiKey = Env.GetString("GROQ");

        // other nodes
        _interactLabel = GetNode<Label>("Label");
        _player = GetNode<Player>("/root/Node/Player");
        _interactLabel.Visible = false; 

        // ai api
        _ai = new GroqApiClient(apiKey);
        var request = new JsonObject
        {
            ["model"] = "mixtral-8x7b-32768",
            ["temperature"] = 0.7,
            ["max_tokens"] = 150,
            ["messages"] = new JsonArray
            {
                new JsonObject
                {
                    ["role"] = "system",
                    ["content"] = "You are a helpful assistant."
                },
                new JsonObject
                {
                    ["role"] = "user",
                    ["content"] = "Write a haiku about artificial intelligence."
                }
            }
        };

        // Convert System.Text.Json.JsonObject to a string
        string requestString = request.ToString();

        // Parse the string into a Newtonsoft.Json.Linq.JObject
        JObject newtonsoftRequest = JObject.Parse(requestString);


        var result = await _ai.CreateChatCompletionAsync(newtonsoftRequest);
        GD.Print(result?["choices"]?[0]?["message"]?["content"]?.ToString());
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
                if (!_spokenTo){
                    ShowInteractUI(true);
                }
				if (_player.Message != string.Empty){
                    _spokenTo = true;
                    ShowInteractUI(false);
                    _RateMessage(_player.Message);
                    _Respond(_player.Message);
                    _player.Message = "";
                }
            }
            else
            {
                // Hide "Press T to interact" UI
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

    private void _RateMessage(string message){
        // _rating = true;

    }

    private void _Respond(string message){
        // _responding = true;
    }
}
