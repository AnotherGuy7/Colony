using Godot;

public class BasicTownNPC03 : RigidBody2D
{
	[Export]
	public string[] dialogue = new string[2];

	[Export]
	public string[] speakerNames = new string[2];

	private bool canBeTalkedTo = false;
	private bool isBeingTalkedTo = false;


	public override void _Process(float delta)
	{
		if (canBeTalkedTo && !isBeingTalkedTo && Input.IsActionJustPressed("Continue") && !GameData.isPlayerTalking)
		{
			DialogueManager.StartDialog(dialogue, speakerNames);
			isBeingTalkedTo = true;
		}
	}


	//Talk stuff
	private void OnTalkAreaEntered(object body)
	{
		if (body == Player.player)
		{
			canBeTalkedTo = true;
		}
	}


	private void OnTalkAreaExited(object body)
	{
		if (body == Player.player)
		{
			canBeTalkedTo = false;
		}
	}
}
