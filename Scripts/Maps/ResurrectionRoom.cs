using Godot;
using System;

public class ResurrectionRoom : Node2D
{
	private void OnRespawnButtonPressed()
	{
		SaveManager.saveManager.LoadGame(GameData.currentPlayerSaveIndex);
	}


	private void OnTitleScreenButtonPressed()
	{
		SceneSwitcher.sceneSwitcher.GotoScene("TitleScreen", 0, "Front");
	}

}
