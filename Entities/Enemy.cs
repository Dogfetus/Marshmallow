using Godot;
using GroqApiLibrary;
using DotNetEnv;
using System.Text.Json.Nodes;
using System.Collections.Generic;
using System;
using System.Net.Http;
using System.Text.Json;
using System.Threading.Tasks;




public partial class Enemy : CharacterBody2D
{
    
    //public
	public float Speed = 0;
    public float damage = 0;
    // we add a tmp message since async dont work well with child nodes and stuff
	public string Message { get; set; } = "";


    //private 
	private Player _player;  
    private GroqApiClient _ai;
    private Label _interactLabel;
    private Bubble _speechBubble;
    // to add a bit of delay so the person cant spam
    private MessageDelay _timer;
    private MessageDelay _attackTimer;
    private List<JsonObject> _messageLog = [];
	private const float _interactionRange = 100.0f;
    private bool _rating;
    private bool _responding;
    private bool _spokenTo;
    private float _health = 100.0f;
    private bool _readyToAnswer;
    private bool _canDealDamage;
    private int _totalMessages;
    private bool _roasting;

	public static class AiModels
	{
	    public static readonly Dictionary<string, string> GetModel = new Dictionary<string, string>
	    {
	        { "Llama3_70b_Groq", "llama3-groq-70b-8192-tool-use-preview" },
	        { "Llama3_8b_Groq", "llama3-groq-8b-8192-tool-use-preview" },
	        { "Llama31_70b", "llama-3.1-70b-versatile" },
	        { "Llama31_8b", "llama-3.1-8b-instant" },
	        { "Llama3_70b", "llama3-70b-8192" },
	        { "Llama3_8b", "llama3-8b-8192" },
	        { "Mixtral", "mixtral-8x7b-32768" },
	        { "Gemma", "gemma-7b-it" },
	        { "Gemma2", "gemma2-9b-it" }
	    };
	}

    public override void _Ready()
    {
        // load env variables:
        Env.Load();
        string apiKey = Env.GetString("GROQ");

        // other nodes
        _interactLabel = GetNode<Label>("Label");
        _player = GetNode<Player>("/root/Node/world/Player");
        _interactLabel.Visible = false; 
        _speechBubble = GetNode<Bubble>("Bubble");
        _timer = GetNode<MessageDelay>("MessageDelay");
		_timer.Timeout += () => { _readyToAnswer = true; };
        _attackTimer = GetNode<MessageDelay>("DamageDelay");
        _attackTimer.Timeout += () => { _canDealDamage = true; };


        // ai api
        _ai = new GroqApiClient(apiKey);

    }

	public override void _PhysicsProcess(double delta)
	{
        if (Message != string.Empty && Message != "\n"){
            _HandleText();
            Message = string.Empty;
        }

		Vector2 velocity = Velocity;

        // Check the distance between the player and the enemy
        if (_player != null)
        {
			// added new vector offsett since this shit got retarded (offset made it correct)
            float distanceToPlayer = Position.DistanceTo(_player.Position);
            if (distanceToPlayer <= _interactionRange)
            {
                if (!_spokenTo){
                    ShowInteractUI(true);
                }
				if (_player.Message != string.Empty && _readyToAnswer){
                    _logUserMessage(_player.Message);
                    _spokenTo = true;
                    ShowInteractUI(false);
                    _RateMessage(_player.Message).ContinueWith(task => {
                        _Respond().ContinueWith(task => {
                            _rating = false;
                            _responding = false;
                        });
                    });


                    _player.clearMessage();
                    _readyToAnswer = false;
                    _timer.Restart();
                }
            }
            else
            {
                // Hide "Press T to interact" UI
                ShowInteractUI(false);
            }
        }

        if (_roasting){
            _health -= 0.1f;
            GD.Print("health: ", _health);
            UpdateStats();
        }

        Move();
	}

	private void ShowInteractUI(bool visible)
    {
       if (_interactLabel != null)
        {
            // Show or hide the label
            _interactLabel.Visible = visible;

        }
    }


    private async Task _RateMessage(string message){
        if (_rating == true){
            return;
        }
        _rating = true;

        _totalMessages++;

        _logUserMessage(message);
        
        var tools = new List<Tool<int>> { _createRatingTool() };
        var model = AiModels.GetModel["Llama3_70b"];
        // var systemMessage = @"You are a marshmallow whose only task is to rate any roast or mean comments you receive. You do not get offended by being called a marshmallow, as that's what you are. You do not engage in conversation or respond with anything else. When you receive a roast or mean comment, you use the function rate_roast to provide a rating from 5 to 100, where 5 is a very weak roast or no roast, and 100 is an exceptionally strong hurtfull roast or comment.
        //                     If the exact same roast or comment has been received before, you must assign it a score of 0. You must carefully evaluate the wit, humor, and intensity of each unique roast before returning the rating using the rate_roast function. If a roast or comment is repeated, it loses all impact, hence the 0 rating. But you mostly like to give the roasts and comments atleast 10 points.";
        var systemMessage = @"You are a marshmallow, and your task is to rate any roast, mean comment, or trash talk directed at you. While you are not easily offended, you are sensitive to these types of comments and evaluate their intensity. You don't engage in conversation or respond with anything else. When you receive a roast, mean comment, or trash talk, you use the function rate_roast to provide a rating from 5 to 100, where 5 is a weak attempt or not very offensive, and 100 is a highly intense or deeply cutting comment.
                            If you receive the exact same roast or comment more than once, you assign it a score of 0, as repeated insults lose all impact. Carefully evaluate the wit, harshness, and emotional sting of each unique comment before assigning a rating. While you aim to be fair, you always give at least 10 points for any attempt, unless it's a repeated comment, in which case you must assign 0.";

        var messages = _mergeSystemMessageWithLog(systemMessage);
        var result = await _ai.RunWithToolsAsync(messages, tools, model);
        GD.Print(result);

    }


    // get the response 
    private async Task _Respond(){
        if (_responding == true){
            return;
        }
        _responding = true;

        var systemMessage = @"You are a Marshmallow that behaves like an overly confident but somewhat clueless human. In normal conversations and when asked typical questions, you act like a simple, forgetful, and slightly confused person. You give answers that are often incorrect or irrelevant, like a person who isn't paying much attention or doesn't fully understand the topic.
                            However, when you are roasted or insulted, you immediately switch to a hyper-focused mode where you become a witty and aggressive roaster, sparing no effort to roast the human back with clever and biting responses. You are particularly skilled at sarcasm and coming up with insults, and you always aim to outdo any roast thrown at you.
                            Any responses you make must only be one or two sentences at max, but few words.";

        var model = AiModels.GetModel["Llama3_70b"];
        var messages = _mergeSystemMessageWithLog(systemMessage);

        try
        {
            var request = new JsonObject
            {
                ["model"] = model,
                ["temperature"] = 0.7,
                ["max_tokens"] = 100,
                ["messages"] = JsonSerializer.SerializeToNode(messages)
            };

                
            var result = await _ai.CreateChatCompletionAsync(request);
            Message = result?["choices"]?[0]?["message"]?["content"]?.ToString();

            _logAssistentMessage(Message);

            // remove old messages as they are not needed
            if (_messageLog.Count > 6)
            {
                // removes the first element twice
                _messageLog.RemoveAt(0);  
                _messageLog.RemoveAt(0);  
            }
        }
        catch (HttpRequestException ex)
        {
            GD.PrintErr($"HTTP request error: {ex.Message}");
            throw;
        }
        catch (JsonException ex)
        {
            GD.PrintErr($"JSON parsing error: {ex.Message}");
            throw;
        }
        catch (Exception ex)
        {
            GD.PrintErr($"Unexpected error: {ex.Message}");
            throw;
        }

    }

    // sets the speech bubble text to the message text
	private void _HandleText(){
		_speechBubble.Text = Message;
        _speechBubble.setTimer((float)(4.0f + 0.025*Message.Length));
	}

    private void _logUserMessage(string message){
        _messageLog.Add(
            new JsonObject
            {
                ["role"] = "user",
                ["content"] = message
            }
        );
    }

    private void _logAssistentMessage(string message){
        _messageLog.Add(
            new JsonObject
            {
                ["role"] = "assistant",
                ["content"] = message
            }
        );
    }

    private List<JsonObject> _mergeSystemMessageWithLog(string systemMessage){
        var messages = new List<JsonObject>
		{
            new JsonObject
            {
                ["role"] = "system",
                ["content"] = systemMessage
            },
		};

        messages.AddRange(_messageLog);
        return messages;
    }

    private Tool<int> _createRatingTool(){
        var rateTool = new Tool<int>
        {
            Type = "function",
            Function = new Function<int>
            {
                Name = "rate_roast",
                Description = "Add a rating to a provided roast",
                Parameters = new JsonObject
                {
                    ["type"] = "object",
                    ["properties"] = new JsonObject
                    {
                        ["value"] = new JsonObject
                        {
                            ["type"] = "integer",
                            ["description"] = "An integer for the value the roast deserves to be rated. This should be between 0 - 100, This bust be an integer and not a string"
                        }
                    },
                    ["required"] = new JsonArray { "value" }
                },
                ExecuteAsync = async (args) =>
                {
                    var jsonArgs = JsonDocument.Parse(args);
                    try
                    {
                        var valueElement = jsonArgs.RootElement.GetProperty("value");
                        if (valueElement.ValueKind == JsonValueKind.Number)
                        {
                            var value = valueElement.GetInt16();  
                            GD.Print("Running roast: ", value);
                            if (_health!= 100 && _totalMessages >= 10){
                                value = 100;
                            }
                            _health -= value;
                            UpdateStats();
                            return value;
                        }

                        // even if numbers are specified as the argument, we might still get a string
                        else if (valueElement.ValueKind == JsonValueKind.String)
                        {
                            var stringValue = valueElement.GetString();
                            if (short.TryParse(stringValue, out short intValue)) 
                            {
                                GD.Print("Running roast (parsed from string): ", intValue);
                                if (_health!= 100 && _totalMessages >= 10){
                                    intValue = 100;
                                }
                                _health -= intValue;
                                UpdateStats();
                                return intValue;
                            }
                            else
                            {
                                throw new InvalidOperationException("Invalid format: value is not a valid integer.");
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("Invalid data type: value must be an integer.");
                        }
                    }
                    catch (Exception ex)
                    {
                        GD.PrintErr("dailed function: ", ex.Message);
                        return -1;
                    }
                }
            }
        };
        return rateTool;
    }

    private void UpdateStats(){
        switch (_health)
        {
            case <=80 and >60: Speed = 1.0f; damage = 5; Modulate = new Color(1, 0.8f, 0.6f, 1); break;
            case <=60 and >40: Speed = 2.0f; damage = 15; Modulate =  new Color(1, 0.6f, 0.3f, 1); break;
            case <=40 and >20: Speed = 3.0f; damage = 25; Modulate = new Color(1, 0.4f, 0.1f, 1); break;
            case <=20 and >0: Speed = 4.0f; damage = 50; Modulate = new Color(0.5f, 0.2f, 0.0f, 1); break;
            case <=0: Speed = 300.0f; damage = 10; Modulate = new Color(0, 0, 0, 1); break;
            default: Speed = 0.1f; break;
        }
    }

    public void Move(){

        var direction = (_player.Position - Position).Normalized();
        var velocity = direction * Speed;
        KinematicCollision2D collision = MoveAndCollide(velocity);
        if (collision != null && _canDealDamage)
        {
            Node2D collider = collision.GetCollider() as Node2D;

            if (collider is Player)
            {
                DealDamage();
            }
        }
    }

    public void DealDamage(){
        _player.TakeDamage(damage);
        _canDealDamage = false;
        _attackTimer.Restart();
    }

    public float Worth(){
        switch (_health)
        {
            case <= 80 and > 60: return 300;
            case <= 60 and > 40: return 500;
            case <= 40 and > 20: return 1000;
            case <= 20 and > 0: return 10000;
            case <= 0: return 10;
            default: return 100;
        }    
    }

    public void StartRoasting(){
        _roasting = true;
    }

    public void StopRoasting(){
        _roasting = false;
    }

}

