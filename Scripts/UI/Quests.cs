using Godot;
using Godot.Collections;

public class Quests : Node
{
	public string questName = "";
	public string questDescription = "";
	public int progress = 0;
	public int maxProgress = 0;
	public int questType = 0;       //1 = collection; 2 = elimination
	public Texture iconSprite;

	public string askerName = "";
	public string locationName = "";
	public string targetNPCName = "";       //self-explanatory, it's what you have to kill for elimination quests

	public static Quests emptyQuest;

	public override void _Ready()
	{
		emptyQuest = new Quests();
		emptyQuest.questName = "None";
		emptyQuest.questDescription = "No quest here";
		emptyQuest.iconSprite = GetIcon("Nothing");
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

	public static bool AddQuest(Quests quest)
	{
		bool questPlaced = false;

		//Checks for if the quest is already active, in which case, it does nothing
		for (int q = 0; q < GameData.activeQuests.Length; q++)
		{
			if (GameData.activeQuests[q].questName == quest.questName)
			{
				questPlaced = true;
				break;
			}
		}

		//Checks for empty quest slots then puts it in there if there's an empty one
		if (!questPlaced)
		{
			for (int q = 0; q < GameData.activeQuests.Length; q++)
			{
				if (GameData.activeQuests[q].questName == "None")
				{
					GameData.activeQuests[q] = quest;
					questPlaced = true;
					break;
				}
			}
		}

		//gameData.EmitSignal(nameof(UpdateQuestProgress));
		return questPlaced;
	}
}
