using Godot;
using Godot.Collections;

public class Quests : Node
{
	public string questName = "";
	public string questDescription = "";
	public string questStarterName = "";        //this should be the name of the guy who initially gave you the quest
	public int progress = 0;
	public int maxProgress = 0;
	public int questType = 0;       //1 = collection; 2 = elimination
	public Texture iconSprite;

	public string askerName = "";
	public string locationName = "";
	public string targetNPCName = "";       //self-explanatory, it's what you have to kill for elimination quests

	public static Dictionary<int, Quests> questsDict = new Dictionary<int, Quests>();

	public override void _Ready()
	{
		Quests emptyQuest = new Quests();
		emptyQuest.questName = "None";
		emptyQuest.questDescription = "No quest here";
		emptyQuest.iconSprite = GetIcon("Nothing");
		questsDict.Add(-1, emptyQuest);

		/*Quests testQuest = new Quests();
		testQuest.questName = "Living Trunk Elimination";
		testQuest.questDescription = "Eliminate " + testQuest.progress + " Living Trunks";
		testQuest.iconSprite = GetIcon("EliminationIcon");
		testQuest.questType = 2;
		questsDict.Add(0, testQuest);*/
	}
	private Texture GetIcon(string textureName)
	{
		Texture texture = (Texture)GD.Load("res://Sprites/UI/Icons/" + textureName + ".png");
		if (texture == null)
		{
			texture = (Texture)GD.Load("res://Sprites/UI/Items/Nothing.png");
		}
		return texture;
	}
}
