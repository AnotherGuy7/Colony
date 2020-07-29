using Godot;
using System;

public class WoodvilleHouses : Node2D
{
	[Export]
	public int positionToSpawnAt;

	private void OnLeaveAreaEntered(object body)
	{
		if (body == Player.player)
		{
			SceneSwitcher.sceneSwitcher.GotoScene(PackedScenes.packedScenesClass.Woodville, positionToSpawnAt, "Front");
		}
	}
}
