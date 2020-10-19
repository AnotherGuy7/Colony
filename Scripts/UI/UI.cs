using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

public class UI : Control
{
	[Export]
	public Texture halfHeart;

	[Export]
	public Texture emptyHeart;

	[Export]
	public Texture fullHeart;

	[Export]
	public Texture inactiveSlot;

	[Export]
	public Texture activeSlot;

	public static Panel savePanel;

	private Panel mapPanel;
	private Camera2D mainMapCam;
	private Camera2D mapCam;
	private TextureRect playerMarker;
	private Panel heartsPanel;
	private Panel inventoryPanel;
	private Panel questPanel;
	private Panel pauseMenu;
	private Label saveSlotLabel;

	private int currentHeart = 0;
	private int questIndex = 0;
	private int saveSlotIndex = 1;

	private List<TextureRect> inventorySlots = new List<TextureRect>();
	private List<TextureRect> hearts = new List<TextureRect>();

	public override void _Ready()
	{
		mapPanel = GetNode<Panel>("Layer1/MapPanel");
		mainMapCam = GetNode<Camera2D>("Layer1/MapPanel/MainMapCam");
		mapCam = GetNode<Camera2D>("Layer1/MapPanel/MapCam");
		playerMarker = GetNode<TextureRect>("Layer1/MapPanel/PlayerMarkerCenter/PlayerMarker");
		heartsPanel = GetNode<Panel>("Layer1/HealthBar");
		inventoryPanel = GetNode<Panel>("Layer1/InventoryPanel");
		questPanel = GetNode<Panel>("Layer1/QuestsPanel");
		pauseMenu = GetNode<Panel>("Layer1/PauseMenu");
		saveSlotLabel = GetNode<Label>("Layer1/SavePanel/SaveSlotLabel");
		savePanel = GetNode<Panel>("Layer1/SavePanel");
		GameData.gameData.Connect(nameof(GameData.UpdateInventorySlotDrawings), this, nameof(UpdateInventoryDrawings));

		foreach (TextureRect heart in heartsPanel.GetChildren())
		{
			hearts.Add(heart);
		}
		foreach (TextureRect slot in inventoryPanel.GetChildren())
		{
			inventorySlots.Add(slot);
		}
	}

	private void UpdateInventoryDrawings()
	{
		for (int i = 0; i < GameData.playerInventory.Length; i++)
		{
			Item item = GameData.playerInventory[i];
			TextureRect currentSlot = inventorySlots[i];
			TextureRect currentSlotItemRect = (TextureRect)inventorySlots[i].GetNode(inventorySlots[i].GetPath() + "/ItemTexture");
			Label currentSlotStack = (Label)inventorySlots[i].GetNode(inventorySlots[i].GetPath() + "/ItemStack");
			currentSlotItemRect.Texture = item.sprite;

			if (item.stack > 1)
				currentSlotStack.Text = item.stack.ToString();
			else
				currentSlotStack.Text = "";

			if (i == GameData.selectedInventorySlot)
				currentSlot.Texture = activeSlot;
			else
				currentSlot.Texture = inactiveSlot;
		}
	}

	public override void _Process(float delta)
	{
		currentHeart = GameData.playerHealth / 2;
		for (int i = 0; i < hearts.Count; i++)		//change this so that it changes upon a signal
		{
			if (i < GameData.playerMaxHealth)
			{
				hearts[i].Visible = true;
			}
			else
			{
				hearts[i].Visible = false;
			}
			if (i > currentHeart)
				hearts[i].Texture = emptyHeart;
			if (i < currentHeart)
				hearts[i].Texture = fullHeart;
			if (i == currentHeart)
			{
				if (GameData.playerHealth % 2f == 1)
				{
					hearts[i].Texture = fullHeart;
				}
				else
				{
					hearts[i].Texture = halfHeart;
				}
			}
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("map"))
		{
			mapPanel.Visible = !mapPanel.Visible;
			mainMapCam.Current = !mainMapCam.Current;
		}
		if (Input.IsActionJustPressed("pause"))
		{
			pauseMenu.Visible = !pauseMenu.Visible;
		}
		if (Input.IsActionJustPressed("quest"))
		{
			UpdateQuestListings();
			questPanel.Visible = !questPanel.Visible;
		}

		if (mapPanel.Visible)
		{
			if (@event is InputEventMouseButton scroll)
			{
				if (scroll.IsPressed())
				{
					if (scroll.ButtonIndex == (int)ButtonList.WheelUp)
					{
						mapCam.Zoom = new Vector2(mapCam.Zoom.x + 0.05f, mapCam.Zoom.y + 0.05f);
					}
					if (scroll.ButtonIndex == (int)ButtonList.WheelDown)
					{
						mapCam.Zoom = new Vector2(mapCam.Zoom.x - 0.05f, mapCam.Zoom.y - 0.05f);
					}
				}
			}
			if (@event is InputEventMouseButton click)
			{
				if (click.IsPressed())
				{
					if (click.ButtonIndex == (int)ButtonList.Left)
					{

					}
				}
			}
		}
	}

	private void UpdateQuestListings()
	{
		Label questName = questPanel.GetNode("QuestName") as Label;
		Label questDescription = questPanel.GetNode("QuestDescription") as Label;
		Label questProvider = questPanel.GetNode("QuestProvider") as Label;
		Label questProgress = questPanel.GetNode("QuestProgress") as Label;
		TextureRect questIcon = questPanel.GetNode("Icon") as TextureRect;

		if (GameData.activeQuests[questIndex].questName != "None")
		{
			questName.Text = GameData.activeQuests[questIndex].questName;
			questDescription.Text = GameData.activeQuests[questIndex].questDescription;
			questIcon.Texture = GameData.activeQuests[questIndex].iconSprite;
			questProvider.Text = "Provider: " + GameData.activeQuests[questIndex].askerName;
			questProgress.Text = "Progress: " + GameData.activeQuests[questIndex].progress + " / " + GameData.activeQuests[questIndex].maxProgress;
			if (GameData.activeQuests[questIndex].progress >= GameData.activeQuests[questIndex].maxProgress)
			{
				questProgress.Text = "Progress: Completed";
			}
		}
		else
		{
			questName.Text = "None";
			questDescription.Text = "No quest occupies this slot. Go talk to someone and start a quest!";
			questIcon.Texture = GameData.activeQuests[questIndex].iconSprite;
			questProvider.Text = "Provider: None";
			questProgress.Text = "Progress: None";
		}
	}

	private void OnQuestBackButtonPressed()
	{
		questIndex--;

		if (questIndex <= 0)
			questIndex = GameData.activeQuests.Length - 1;

		UpdateQuestListings();
	}


	private void OnQuestForwardButtonPressed()
	{
		questIndex++;

		if (questIndex > GameData.activeQuests.Length - 1)
			questIndex = 1;

		UpdateQuestListings();
	}

	private void OnContinueButtonPressed()
	{
		GetTree().Paused = false;
		pauseMenu.Visible = false;
	}


	private void OnSettingsButtonPressed()
	{
		// Replace with function body.
	}


	private void OnBackToMenuButtonPressed()
	{
		SceneSwitcher.sceneSwitcher.GotoScene("Title", 1, "Back");
	}

	private void OnSaveButtonPressed()
	{
		SaveManager.saveManager.SaveGame(saveSlotIndex);
		GameData.gameData.EmitSignal(nameof(GameData.SavedGame));
		savePanel.Visible = false;
	}

	private void OnSaveSlotPlusButtonPressed()
	{
		saveSlotIndex++;
		if (saveSlotIndex > 10)
		{
			saveSlotIndex = 0;
		}
		saveSlotLabel.Text = saveSlotIndex.ToString();
	}

	private void OnSaveSlotMinusButtonPressed()
	{
		saveSlotIndex--;
		if (saveSlotIndex < 0)
		{
			saveSlotIndex = 10;
		}
		saveSlotLabel.Text = saveSlotIndex.ToString();
	}
}
