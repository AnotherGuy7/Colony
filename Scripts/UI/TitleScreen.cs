using Godot;
using Godot.Collections;
using System;

public class TitleScreen : Control
{
	private Panel settingsPanel;
	private Panel savesPanel;
	private Label resolutionNumber;
	private string filePath = "user://savegame.save";

	private Vector2[] resolutionsArray = new Vector2[7] { new Vector2(256f, 150f), Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
	private int resolutionsArrayIndex = 0;
	private int saveSlotIndex = 1;
	private string[] locationsArray = new string[GameData.MaxSaveSlots];

	public override void _Ready()
	{
		settingsPanel = GetNode<Panel>("SettingsPanel");
		savesPanel = GetNode<Panel>("SavesPanel");
		resolutionNumber = GetNode<Label>("SettingsPanel/ResolutionNumberLabel");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey)
		{
			InputEventKey keyPressed = @event as InputEventKey;
			if (keyPressed.Scancode == (int)KeyList.Escape)
			{
				if (settingsPanel.Visible)
				{
					settingsPanel.Visible = false;
				}
			}
		}
	}

	//Signals
	private void OnPlayButtonPressed()
	{
		savesPanel.Visible = true;
		LoadSaveData(saveSlotIndex);
	}

	private void OnLoadGameButtonPressed()
	{
		string locationName = locationsArray[saveSlotIndex - 1];
		if (locationName != "None")
		{
			SceneSwitcher.sceneSwitcher.GotoScene(locationName, 1, "Front");
			SaveManager.saveManager.LoadGame(saveSlotIndex);
			GameData.currentPlayerSaveIndex = saveSlotIndex;
		}
		else
		{
			SceneSwitcher.sceneSwitcher.GotoScene("Woodville", 1, "Front");
			SaveManager.saveManager.LoadGame(saveSlotIndex);
		}
	}

	private void OnSaveIndexMinusButtonPressed()
	{
		saveSlotIndex--;
		if (saveSlotIndex <= 0)
		{
			saveSlotIndex = GameData.MaxSaveSlots;
		}
		LoadSaveData(saveSlotIndex);
	}


	private void OnSaveIndexPlusButtonPressed()
	{
		saveSlotIndex++;
		if (saveSlotIndex > GameData.MaxSaveSlots)
		{
			saveSlotIndex = 1;
		}
		LoadSaveData(saveSlotIndex);
	}

	private void OnSettingsButtonPressed()
	{
		settingsPanel.Visible = true;
	}

	private void OnLowerResPresse()
	{
		resolutionsArrayIndex -= 1;
		if (resolutionsArrayIndex < 0)
		{
			resolutionsArrayIndex = resolutionsArray.Length - 1;
		}
		UpdateResText();
	}

	private void OnHigherResPressed()
	{
		resolutionsArrayIndex += 1;
		if (resolutionsArrayIndex < resolutionsArray.Length)
		{
			resolutionsArrayIndex = 0;
		}
		UpdateResText();
	}


	private void OnFullscreenPressed()
	{
		OS.WindowFullscreen = !OS.WindowFullscreen;
	}

	//Extra methods
	private void UpdateResText()
	{
		resolutionNumber.Text = resolutionsArray[resolutionsArrayIndex].x + " x " + resolutionsArray[resolutionsArrayIndex].y;
	}

	private void LoadSaveData(int index)
	{
		File saveFile = new File();
		string newFilePath = "user://savegame" + index.ToString() + ".save";

		Panel savePanel = GetNode<Panel>("SavesPanel/SavePanel");

		savePanel.GetNode<Label>("SaveIndexLabel").Text = saveSlotIndex.ToString();
		savePanel.GetNode<Label>("SaveLabel").Text = "Save " + saveSlotIndex + ":";

		if (saveFile.FileExists(newFilePath))
		{
			if (!saveFile.IsOpen())
				saveFile.Open(newFilePath, File.ModeFlags.Read);

			Dictionary<string, object> saveData = new Dictionary<string, object>((Dictionary)JSON.Parse(saveFile.GetLine()).Result);        //Cloning the dictionary stored in the save

			saveFile.Close();

			savePanel.GetNode<Label>("MoneyLabel").Text = "Money: " + saveData["Money"].ToString();
			locationsArray[saveSlotIndex - 1] = saveData["Location"].ToString();
			savePanel.GetNode<Label>("LocationLabel").Text = "Location: " + locationsArray[saveSlotIndex - 1];

			for (int i = 0; i < GameData.playerInventory.Length; i++)
			{
				TextureRect slot = savePanel.GetNode<TextureRect>("InventorySlot" + (i + 1));
				TextureRect slotSprite = savePanel.GetNode<TextureRect>("InventorySlot" + (i + 1) + "/ItemTexture");
				int itemType = int.Parse(saveData["ItemType" + (i + 1)].ToString());
				slotSprite.Texture = Item.itemList[itemType].sprite;
				slot.Visible = slotSprite.Visible = true;
			}
		}
		else
		{
			savePanel.GetNode<Label>("MoneyLabel").Text = "";
			savePanel.GetNode<Label>("LocationLabel").Text = "None";

			for (int i = 0; i < GameData.playerInventory.Length; i++)
			{
				TextureRect slot = savePanel.GetNode<TextureRect>("InventorySlot" + (i + 1));
				TextureRect slotSprite = savePanel.GetNode<TextureRect>("InventorySlot" + (i + 1) + "/ItemTexture");
				slot.Visible = slotSprite.Visible = false;
			}
		}
	}

}
