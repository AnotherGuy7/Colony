using Godot;
using System;

public class SceneSwitchArea : Area2D
{
	[Export]
	public string locationName;

	[Export]
	public string direction;

	[Export]
	public int spawnPoint;

	[Export]
	public bool showFlag;

	[Export]
	public string modifiedLocationName = "";

	[Export]
	public bool goInvisible = false;

	[Export]
	public bool keepWalking = false;

	private void OnSwitchAreaBodyEntered(object body)
	{
		if (body == Player.player)
		{
			if (goInvisible)
            {
				Player.player.Visible = false;
            }
			if (keepWalking)
            {
				Player.moveDuringTransition = true;
            }
			SceneSwitcher.sceneSwitcher.GotoScene(locationName, spawnPoint, direction, showFlag, modifiedLocationName);
		}
	}
}
