using Godot;

public class Bed : Area2D
{
	[Export]
	public string[] dialogue = new string[1];

	[Export]
	public string[] speakerNames = new string[1];

	private bool canAccessBed = false;

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("Continue") && canAccessBed)
		{
			DialogueManager.StartSaveDialog(dialogue, speakerNames);
			GameData.isPlayerTalking = true;
		}
	}

	private void OnBedBodyEntered(object body)
	{
		if (body == Player.player)
		{
			canAccessBed = true;
		}
	}

	private void OnBedBodyExited(object body)
	{
		if (body == Player.player)
		{
			canAccessBed = false;
		}
	}

}
