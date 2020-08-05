using Godot;
using System;

public class TitleScreen : Node2D
{
	private void OnPlayButtonPressed()
	{
		SceneSwitcher.sceneSwitcher.GotoScene(PackedScenes.packedScenesClass.Woodville, 1, "Front");
	}
	
	
	private void OnSettingsButtonPressed()
	{
		// Replace with function body.
	}
}
