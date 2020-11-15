using Godot;
using System;

public class BullitenBoard : Sprite
{
	[Export]
	public string[] messages = new string[1];

	[Export]
	public string[] names = new string[1];

	private bool canRead = false;

	public override void _Ready()
	{
		
	}

	public override void _Process(float delta)
	{
		if (canRead && Input.IsActionJustPressed("Continue") && !GameData.isPlayerTalking)
		{
			DialogueManager.StartDialog(messages, names);
			GameData.isPlayerTalking = true;
		}

	}

	private void OnReadingAreaBodyEntered(object body)
	{
		if (body == Player.player)
		{
			canRead = true;
		}
	}


	private void OnReadingAreaBodyExited(object body)
	{
		if (body == Player.player)
		{
			canRead = false;
		}
	}
}
