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

	public void SaveGame()
	{
		File saveFile = new File();

		if (!saveFile.IsOpen())
			saveFile.Open(filePath, File.ModeFlags.Write);

		Dictionary<string, object> thingsToSave = new Dictionary<string, object>
		{
			{ "Health", GameData.playerHealth },
			{ "Money", GameData.playerCurrency },
			{ "Inventory", GameData.playerInventory },
			{ "Quests", GameData.activeQuests },
			{ "PosX", Player.player.GlobalPosition.x },
			{ "PosY", Player.player.GlobalPosition.y }
		};

		saveFile.StoreLine(JSON.Print(thingsToSave));
		saveFile.Close();
	}

	public void LoadGame()
	{
		File saveFile = new File();

		if (!saveFile.FileExists(filePath))
			return;

		if (!saveFile.IsOpen())
			saveFile.Open(filePath, File.ModeFlags.Read);

		Dictionary<string, object> saveData = new Dictionary<string, object>((Dictionary)JSON.Parse(saveFile.GetLine()).Result);        //Cloning the dictionary stored in the save

		saveFile.Close();

		GD.Print(saveData);

		var health = saveData["Health"];        //oddly enough, casting as a type breaks the game?
		var money = saveData["Money"];
		var items = saveData["Inventory"];
		var quests = saveData["Quests"];
		var posX = saveData["PosX"];
		var posY = saveData["PosY"];

		GD.Print(health);

		/*GameData.playerHealth = (int)health;
		GameData.playerCurrency = (int)money;
		GameData.playerInventory = (Item[])items;
		GameData.activeQuests = (Quests[])quests;
		GameData.playerSavedPosition = new Vector2((float)posX, (float)posY);
		*/
	}
}
