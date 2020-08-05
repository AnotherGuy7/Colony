using Godot;
using System;

public class Woodville : Node2D
{
	private void OnAreaToQuarryBodyEntered(object body)
	{
		if (body == Player.player)
		{
			SceneSwitcher.sceneSwitcher.GotoScene(PackedScenes.packedScenesClass.WoodvilleQuarry, 1, "Back");
		}
	}
}
