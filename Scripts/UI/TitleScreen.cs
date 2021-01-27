using Godot;
using Godot.Collections;
using System;

public class TitleScreen : Control
{
	private Panel settingsPanel;
	private Panel savesPanel;
	private Label resolutionNumber;

	private Vector2[] resolutionsArray = new Vector2[3] { new Vector2(256f, 150f), new Vector2(512f, 300f), new Vector2(1280f, 720f) };
	private int resolutionsArrayIndex = 0;
	private int saveSlotIndex = 1;
	private bool saveExists = false;
	private bool settingsPanelOut = false;
	private bool savesPanelOut = false;
	private string[] locationsArray = new string[GameData.MaxSaveSlots];

	private Texture checkBoxDisabled;
	private Texture checkBoxActive;
	private TextureRect fullscreenCheckBox;
	private AnimationPlayer titleAnimPlayer;

	public override void _Ready()
	{
		settingsPanel = GetNode<Panel>("SettingsPanel");
		savesPanel = GetNode<Panel>("SavesPanel");
		resolutionNumber = GetNode<Label>("SettingsPanel/ResolutionNumberLabel");
		fullscreenCheckBox = GetNode<TextureRect>("SettingsPanel/FullScreenCheckBox");
		titleAnimPlayer = GetNode<AnimationPlayer>("TitleAnimPlayer");

		checkBoxDisabled = (Texture)GD.Load("res://Sprites/UI/SettingsJournalCheckBoxDisabled.png");
		checkBoxActive = (Texture)GD.Load("res://Sprites/UI/SettingsJournalCheckBoxActive.png");
		OS.WindowSize = resolutionsArray[2];

	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey)
		{
			InputEventKey keyPressed = @event as InputEventKey;
			if (keyPressed.Scancode == (int)KeyList.Escape)
			{
				if (settingsPanelOut)
				{
					settingsPanelOut = false;
					titleAnimPlayer.PlayBackwards("SettingsTransition");
				}
				if (savesPanelOut)
				{
					savesPanelOut = false;
					titleAnimPlayer.PlayBackwards("SavesTransition");
				}
			}
		}
	}

	//Signals
	private void OnPlayButtonPressed()
	{
		if (!savesPanelOut)
		{
			savesPanelOut = true;
			titleAnimPlayer.Play("SavesTransition");
		}
		LoadSaveData(saveSlotIndex);
	}

	private void OnLoadGameButtonPressed()
	{
		string locationName = locationsArray[saveSlotIndex - 1];
		if (saveExists)
		{
			SaveManager.LoadGame(saveSlotIndex);
			GameData.currentPlayerSaveIndex = saveSlotIndex;
			SceneSwitcher.sceneSwitcher.GotoScene(locationName, 1, "Front");
		}
		else
		{
			SceneSwitcher.sceneSwitcher.GotoScene("Woodville", 1, "Front");
			SaveManager.LoadGame(saveSlotIndex);
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
		if (!settingsPanelOut)
		{
			settingsPanelOut = true;
			titleAnimPlayer.Play("SettingsTransition");
		}
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
		if (resolutionsArrayIndex >= resolutionsArray.Length)
		{
			resolutionsArrayIndex = 0;
		}
		UpdateResText();
	}


	private void OnFullscreenPressed()
	{
		OS.WindowFullscreen = !OS.WindowFullscreen;
		if (OS.WindowFullscreen)
		{
			fullscreenCheckBox.Texture = checkBoxActive;
		}
		else
		{
			fullscreenCheckBox.Texture = checkBoxDisabled;
		}
	}

	//Extra methods
	private void UpdateResText()
	{
		resolutionNumber.Text = resolutionsArray[resolutionsArrayIndex].x + " x " + resolutionsArray[resolutionsArrayIndex].y;
		OS.WindowSize = resolutionsArray[resolutionsArrayIndex];
	}

	private void LoadSaveData(int index)
	{
		File saveFile = new File();
		string newFilePath = SaveManager.GetSavePath(index);

		Panel savePanel = GetNode<Panel>("SavesPanel/SavePanel");

		savePanel.GetNode<Label>("SaveIndexLabel").Text = saveSlotIndex.ToString();
		savePanel.GetNode<Label>("SaveLabel").Text = "Save " + saveSlotIndex + ":";

		saveExists = saveFile.FileExists(newFilePath);
		if (saveExists)
		{
			if (!saveFile.IsOpen())
				saveFile.Open(newFilePath, File.ModeFlags.Read);

			Dictionary<string, object> saveData = new Dictionary<string, object>((Dictionary)JSON.Parse(saveFile.GetLine()).Result);        //Cloning the dictionary stored in the save

			saveFile.Close();

			savePanel.GetNode<Label>("MoneyLabel").Text = "Money: " + saveData["Money"].ToString();
			locationsArray[saveSlotIndex - 1] = saveData["Location"].ToString();
			savePanel.GetNode<Label>("LocationLabel").Text = "Location: " + locationsArray[saveSlotIndex - 1];

			for (int i = 0; i < GameData.MaxInventorySlots; i++)
			{
				TextureRect slotSprite = savePanel.GetNode<TextureRect>("ItemTexture" + (i + 1));
				int itemType = int.Parse(saveData["ItemType" + (i + 1)].ToString());
				slotSprite.Texture = Item.itemsDict[itemType].sprite;
				slotSprite.Visible = true;
			}
		}
		else
		{
			savePanel.GetNode<Label>("MoneyLabel").Text = "";
			savePanel.GetNode<Label>("LocationLabel").Text = "None";

			for (int i = 0; i < GameData.MaxInventorySlots; i++)
			{
				TextureRect slotSprite = savePanel.GetNode<TextureRect>("ItemTexture" + (i + 1));
				slotSprite.Visible = false;
			}
		}
	}

}
