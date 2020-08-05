using Godot;
using System;

public class Quarry : Node2D
{
	private void OnAreaToWoodvilleEntered(object body)
	{
		SceneSwitcher.sceneSwitcher.GotoScene(PackedScenes.packedScenesClass.Woodville, 4, "Front");
	}
}
