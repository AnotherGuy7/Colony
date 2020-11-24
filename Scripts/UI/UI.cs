using Godot;
using System;
using System.Collections.Generic;

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

	public static UI ui;

	//Things that are referenced in other files via UI.ui
	public Panel savePanel;
	public AnimationPlayer uiAnimPlayer;

	//Things we only need to access here
	private Panel mapPanel;
	private Camera2D mapCam;
	private Viewport mapViewport;
	private TextureRect playerMarker;
	private Panel heartsPanel;
	private Panel inventoryPanel;
	private Panel questPanel;
	private Panel pauseMenu;
	private Panel tooltipPanel;
	private Label saveSlotLabel;
	private Button questBackButton;
	private Button questForwardButton;
	private AudioStreamPlayer mapInSFX;
	private AudioStreamPlayer mapOutSFX;

	private int currentHeart = 0;
	private int questIndex = 0;
	private int saveSlotIndex = 1;

	public static bool mapPanelActive = false;
	private bool questJournalActive = false;

	private List<TextureRect> inventorySlots = new List<TextureRect>();
	private List<TextureRect> hearts = new List<TextureRect>();

	public override void _Ready()
	{
		mapPanel = GetNode<Panel>("Layer1/MapPanel");
		mapCam = GetNode<Camera2D>("Layer1/MapPanel/ViewportContainer/MapViewport/MapCam");
		mapViewport = GetNode<Viewport>("Layer1/MapPanel/ViewportContainer/MapViewport");
		playerMarker = GetNode<TextureRect>("Layer1/MapPanel/ViewportContainer/MapViewport/PlayerMarkerCenter/PlayerMarker");
		heartsPanel = GetNode<Panel>("Layer1/HealthBar");
		inventoryPanel = GetNode<Panel>("Layer1/InventoryPanel");
		questPanel = GetNode<Panel>("Layer1/QuestsPanel");
		pauseMenu = GetNode<Panel>("Layer3/PauseMenu");
		tooltipPanel = GetNode<Panel>("Layer1/TooltipPanel");
		saveSlotLabel = GetNode<Label>("Layer3/SavePanel/SaveSlotLabel");
		savePanel = GetNode<Panel>("Layer3/SavePanel");
		uiAnimPlayer = GetNode<AnimationPlayer>("UIAnimPlayer");
		mapInSFX = GetNode<AudioStreamPlayer>("Layer1/MapPanel/MapInSFX");
		mapOutSFX = GetNode<AudioStreamPlayer>("Layer1/MapPanel/MapOutSFX");
		ui = this;
		GameData.gameData.Connect(nameof(GameData.UpdateInventorySlotDrawings), this, nameof(UpdateInventoryDrawings));

		questBackButton =  questPanel.GetNode<Button>("QuestBackButton");
		questForwardButton = questPanel.GetNode<Button>("QuestForwardButton");


		//Precautions
		mapViewport.SetProcessInput(false);
		mapViewport.GuiDisableInput = false;

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
		for (int i = 0; i < GameData.MaxInventorySlots; i++)
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
		for (int i = 0; i < hearts.Count; i++)      //change this so that it changes upon a signal
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

		if (tooltipPanel.Visible)
		{
			tooltipPanel.RectPosition = ui.GetLocalMousePosition() + new Vector2(132f, 78f);
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("map") && !uiAnimPlayer.IsPlaying() && !mapPanelActive)
		{
			uiAnimPlayer.Play("MapTransitionIn");
			mapViewport.SetProcessInput(true);
			mapViewport.HandleInputLocally = true;
			mapViewport.GuiDisableInput = false;
		}
		if (Input.IsActionJustPressed("pause"))
		{
			pauseMenu.Visible = !pauseMenu.Visible;
		}
		if (Input.IsActionJustPressed("quest"))
		{
			UpdateQuestListings();
			if (!questJournalActive)
			{
				uiAnimPlayer.Play("QuestJournalIn");
			}
			else
			{
				uiAnimPlayer.Play("QuestJournalOut");

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

		questForwardButton.Visible = true;
		questForwardButton.Disabled = false;

		if (questIndex <= 0)
		{
			questBackButton.Visible = false;
			questBackButton.Disabled = true;
		}


		UpdateQuestListings();
	}


	private void OnQuestForwardButtonPressed()
	{
		questIndex++;

		questBackButton.Visible = true;
		questBackButton.Disabled = false;

		if (questIndex >= GameData.activeQuests.Length - 1)
		{
			questForwardButton.Visible = false;
			questForwardButton.Disabled = true;
		}

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
		SaveManager.SaveGame(saveSlotIndex);
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

	private void OnUIAnimPlayerAnimationFinished(String anim_name)
	{
		switch (anim_name)
		{
			case "MapTransitionIn":
				mapPanelActive = true;
				break;
			case "MapTransitionOut":
				mapPanelActive = false;
				break;
			case "QuestJournalIn":
				questJournalActive = true;
				break;
			case "QuestJournalOut":
				questJournalActive = false;
				break;
		}
		
	}

	public static void ControlTooltipBox(bool visibility, string name)
	{
		ui.tooltipPanel.GetNode<Label>("NameLabel").Text = name;
		ui.tooltipPanel.Visible = visibility;
	}
}
