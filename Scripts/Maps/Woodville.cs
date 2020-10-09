using Godot;
using System;

public class Woodville : Node2D
{
	private void OnAreaToQuarryBodyEntered(object body)
	{
		if (body == Player.player)
		{
			SceneSwitcher.sceneSwitcher.GotoScene("Quarry", 1, "Back", true);
		}
	}
}
