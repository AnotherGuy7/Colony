using Godot;
using System;

public class GameData : Node2D
{
	[Signal]
	public delegate void SwitchedMaps(int positionToSpawnAt, string direction = "Front");		//this is sent after a map change

	[Signal]
	public delegate void FinishedSpawning(bool entering);       //this is sent after the player has finished loading into a map and is already in the proper position and direction

	public static GameData gameData;
	public static YSort mapYSort;

	public static int playerHealth = 4;
	public static int playerDamage = 1;

	public static bool playerHurt = false;      //I could also emit a signal... but this is easier and a signal would require me to check anyway
	public static bool isPlayerTalking = false;

	public override void _Ready()
	{
		gameData = this;
	}

	public static void HurtPlayer(int damage)
	{
		playerHealth -= damage;
		playerHurt = true;
	}
}
