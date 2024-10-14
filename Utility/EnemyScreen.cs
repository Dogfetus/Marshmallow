using Godot;
using System;
using System.Collections.Generic;


public partial class EnemyScreen : Control
{
	private RichTextLabel _text;
	private TextEdit _playerText;
	// all ai models we have to work with:
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

	// Called when the node enters the scene tree for the first time.
	public override void _Ready()
	{
 		_text  = GetNode<RichTextLabel>("RichTextLabel");
		_playerText = GetNode<TextEdit>("TextEdit");
	}

	// Called every frame. 'delta' is the elapsed time since the previous frame.
	public override void _Process(double delta)
	{
		
	}

	private void OnSpeakButtonPressed(){
		GD.Print(_playerText.Text);
		_text.Text = "nice";
	}

	// this function will use the groq api to make an ai rate the incomming roast from the player
	private float RateRoast()
	{
		return 0.0f;
	}

	// this function will write a response to the player ussing the groq api
	private string AnswerRoast()
	{
		return "";
	}
}
