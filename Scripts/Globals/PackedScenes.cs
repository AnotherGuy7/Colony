using Godot;
using Godot.Collections;

public class PackedScenes : Node2D
{
	//Maps scenes
	[Export]
	public PackedScene TitleScreen;

	[Export]
	public PackedScene Woodville;

	[Export]
	public PackedScene WoodvilleHouse1;

	[Export]
	public PackedScene WoodvilleHouse2;

	[Export]
	public PackedScene WoodvilleForest1;

	[Export]
	public PackedScene LumberJackHouse;

	[Export]
	public PackedScene WoodvilleMerchant;

	[Export]
	public PackedScene WoodvilleBrewer;

	[Export]
	public PackedScene deathScreen;


	//Misc scenes
	[Export]
	public PackedScene deathClouds;

	public static Dictionary<string, PackedScene> scenesDict = new Dictionary<string, PackedScene>();

	public override void _Ready()
	{
		scenesDict.Add("Title", TitleScreen);
		scenesDict.Add("Woodville", Woodville);
		scenesDict.Add("WoodvilleHouse1", WoodvilleHouse1);
		scenesDict.Add("WoodvilleHouse2", WoodvilleHouse2);
		scenesDict.Add("WoodvilleForest1", WoodvilleForest1);
		scenesDict.Add("LumberjackHouse", LumberJackHouse);
		scenesDict.Add("WoodvilleMerchant", WoodvilleMerchant);
		scenesDict.Add("WoodvilleBrewer", WoodvilleBrewer);
		scenesDict.Add("DeathScreen", deathScreen);

		scenesDict.Add("DeathClouds", deathClouds);
	}
}
