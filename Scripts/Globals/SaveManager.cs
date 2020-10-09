using Godot;
using Godot.Collections;

public class SaveManager : Node
{
	public static SaveManager saveManager;
	private string filePath = "user://savegame.save";

	public override void _Ready()
	{
		saveManager = this;
	}

	public void SaveGame(int saveIndex)
	{
		File saveFile = new File();

		if (!saveFile.IsOpen())
			saveFile.Open("user://savegame" + saveIndex.ToString() + ".save", File.ModeFlags.Write);

		Dictionary<string, object> thingsToSave = new Dictionary<string, object>		//In here we save the stuff that should be saved easily
		{
			{ "Health", GameData.playerHealth },
			{ "Money", GameData.playerCurrency },
			{ "PosX", Player.player.GlobalPosition.x },
			{ "PosY", Player.player.GlobalPosition.y },
			{ "Location", GameData.playerLocation }
		};

		//Stuff that's a bit harder to save
		for (int i = 0; i < GameData.playerInventory.Length; i++)
		{
			thingsToSave.Add("ItemType" + (i + 1), GameData.playerInventory[i].type);
			thingsToSave.Add("ItemStack" + (i + 1), GameData.playerInventory[i].stack);
		}

		for (int q = 0; q < GameData.activeQuests.Length; q++)
		{
			Quests quest = GameData.activeQuests[q];
			thingsToSave.Add("Quest" + q + "Name", quest.Name);
			thingsToSave.Add("Quest" + q + "Desc", quest.questDescription);
			thingsToSave.Add("Quest" + q + "MaxProgress", quest.maxProgress);
			thingsToSave.Add("Quest" + q + "Progress", quest.progress);
		}

		saveFile.StoreLine(JSON.Print(thingsToSave));
		saveFile.Close();
	}

	public void LoadGame(int saveSlotIndex)
	{
		File saveFile = new File();
		string newFilePath = filePath + saveSlotIndex.ToString();

		if (!saveFile.FileExists(newFilePath))
			return;

		if (!saveFile.IsOpen())
			saveFile.Open(newFilePath, File.ModeFlags.Read);

		Dictionary<string, object> saveData = new Dictionary<string, object>((Dictionary)JSON.Parse(saveFile.GetLine()).Result);        //Cloning the dictionary stored in the save

		saveFile.Close();

		GameData.playerHealth = int.Parse(saveData["Health"].ToString());
		GameData.playerCurrency = int.Parse(saveData["Money"].ToString());
		GameData.playerLocation = saveData["Location"].ToString();

		for (int i = 0; i < GameData.playerInventory.Length; i++)
		{
			int itemType = int.Parse(saveData["ItemType" + (i + 1)].ToString());
			int itemStack = int.Parse(saveData["ItemStack" + (i + 1)].ToString());
			GameData.playerInventory[i] = Item.itemList[itemType];
			GameData.playerInventory[i].stack = itemStack;
		}

		for (int q = 0; q < GameData.activeQuests.Length; q++)
		{
			GameData.activeQuests[q].questName = saveData["Quest" + q + "Name"].ToString();
			GameData.activeQuests[q].questDescription = saveData["Quest" + q + "Desc"].ToString();
			GameData.activeQuests[q].maxProgress = int.Parse(saveData["Quest" + q + "MaxProgress"].ToString());
			GameData.activeQuests[q].maxProgress = int.Parse(saveData["Quest" + q + "Progress"].ToString());
		}

		GameData.playerSavedPosition = new Vector2((float)saveData["PosX"], (float)saveData["PosY"]);
	}
}
