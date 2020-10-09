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
	public string modifiedLocationName;

	private void OnSwitchAreaBodyEntered(object body)
	{
		if (body == Player.player)
		{
			SceneSwitcher.sceneSwitcher.GotoScene(locationName, spawnPoint, direction, showFlag, modifiedLocationName);
		}
	}
}
