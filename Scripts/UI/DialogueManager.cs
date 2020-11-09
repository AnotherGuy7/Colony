using Godot;
using System;
using System.Collections.Generic;

public class DialogueManager : Control
{
	private static DialogueManager dialogManager;
	private Label dialogueText;
	private Label nameLabel;
	private Panel dialogPanel;
	private AudioStreamPlayer talkSound;

	public static List<string> speakerNames = new List<string>();
	public static List<string> activeDialog = new List<string>();
	public static List<int> shopArray = new List<int>();

	//Talking stuff
	private string currentText = "";
	private string currentName = "";
	private int dialogIndex = -1;
	private int visibleCharacters = 0;

	//Quest reward stuff
	private int moneyAmount = 0;
	private Item itemToGive = null;
	private int itemToGivestack = 0;

	//Save Panel stuff
	private bool saveDialog = false;

	private Random rand = new Random();

	public override void _Ready()
	{
		dialogManager = this;
		dialogueText = GetNode<Label>("Layer2/DialogPanel/DialogueText");
		nameLabel = GetNode<Label>("Layer2/DialogPanel/NameLabel");
		dialogPanel = GetNode<Panel>("Layer2/DialogPanel");
		talkSound = GetNode<AudioStreamPlayer>("TalkSound");
		GameData.gameData.Connect(nameof(GameData.SavedGame), this, nameof(ForceCloseSaveDialogue));
	}

	public override void _Process(float delta)
	{
		if (GameData.isPlayerTalking)
		{
			if (visibleCharacters < currentText.Length)
			{
				visibleCharacters += 1;
				//talkSound.PitchScale = rand.Next(97, 103) / 100f;
				//talkSound.Play();
			}
			if (Input.IsActionJustPressed("Continue"))
			{
				if (visibleCharacters < currentText.Length)
				{
					visibleCharacters = currentText.Length;
				}
				else
				{
					dialogIndex++;
					if (dialogIndex > activeDialog.Count - 1)
					{
						if (saveDialog)     //If it's the save prompt then just undo the addition, make the save panel show up, and stop updating text
						{
							UI.ui.savePanel.Visible = true;
						}
						else
						{
							EndDialog();
						}
						return;
					}
					currentText = activeDialog[dialogIndex];
					dialogueText.Text = currentText;
					currentName = speakerNames[dialogIndex];
					nameLabel.Text = currentName;
					visibleCharacters = 0;
				}
			}
			dialogueText.VisibleCharacters = visibleCharacters;
		}
	}

	private void EndDialog()
	{
		dialogIndex = -1;
		activeDialog.Clear();
		speakerNames.Clear();
		saveDialog = false;
		GameData.playerCurrency += moneyAmount;
		if (itemToGive != null)
			Item.AddItemToInventory(itemToGive, itemToGivestack);
		moneyAmount = 0;
		itemToGive = null;
		itemToGivestack = 0;
		dialogPanel.Visible = false;
		GameData.isPlayerTalking = false;
	}

	public static void StartDialog(string[] dialogArray, string[] nameArray)
	{
		dialogManager.InitializeDialogArrays(dialogArray, nameArray);
		dialogManager.dialogIndex = 0;
		dialogManager.dialogueText.Text = dialogManager.currentText = dialogArray[0];
		dialogManager.nameLabel.Text = nameArray[0];
		dialogManager.dialogPanel.Visible = true;
	}

	/*public static void StartDialogWithQuest(string[] dialogArray, string[] nameArray, int questKey, string questsFullMessage, int questAmount)
	{
		for (int t = 0; t < dialogArray.Length; t++)
		{
			activeDialog.Add(dialogArray[t]);
			speakerNames.Add(nameArray[t]);
		}
		if (!GameData.AddQuest(Quests.questsDict[questKey], questAmount))		//tries to add the quest, and if it's full it's gonna add the quest failed line to the dialog
		{
			activeDialog.Add(questsFullMessage);
		}
		dialogManager.dialogIndex = 0;
		dialogManager.dialogueText.Text = dialogArray[0];
		dialogManager.nameLabel.Text = nameArray[0];
		dialogManager.dialogPanel.Visible = true;
	}*/

	public static void StartDialogWithQuest(string[] dialogArray, string[] nameArray, string questName, string questDesc, int questType, string targetNPCName, string providerName, string questsFullMessage, int questMaxProgress)
	{
		dialogManager.InitializeDialogArrays(dialogArray, nameArray);

		for (int q = 0; q < GameData.activeQuests.Length; q++)       //This is so that upon loading, the array has object values while will be modified on save, and it's above all else since it has to happen
		{
			Quests.emptyQuest = new Quests();
			Quests.emptyQuest.questName = "None";
			Quests.emptyQuest.questDescription = "No quest here";
			GameData.activeQuests[q] = Quests.emptyQuest;
		}

		Quests newQuest = new Quests();
		newQuest.questName = questName;
		newQuest.questDescription = questDesc;
		newQuest.questType = questType;
		newQuest.progress = 0;
		newQuest.maxProgress = questMaxProgress;
		newQuest.targetNPCName = targetNPCName;
		newQuest.askerName = providerName;

		if (!Quests.AddQuest(newQuest))       //tries to add the quest, and if it's full it's gonna add the quest failed line to the dialog
		{
			activeDialog.Add(questsFullMessage);
			speakerNames.Add(nameArray[0]);
		}

		dialogManager.dialogIndex = 0;
		dialogManager.dialogueText.Text = dialogManager.currentText = dialogArray[0];
		dialogManager.nameLabel.Text = nameArray[0];
		dialogManager.dialogPanel.Visible = true;
	}

	public static void StartDialogWithReward(string[] dialogArray, string[] nameArray, int moneyAmount = 0, Item itemToGive = null, int itemStack = 0)
	{
		dialogManager.InitializeDialogArrays(dialogArray, nameArray);

		dialogManager.moneyAmount = moneyAmount;
		dialogManager.itemToGive = itemToGive;
		dialogManager.itemToGivestack = itemStack;

		dialogManager.dialogIndex = 0;
		dialogManager.dialogueText.Text = dialogManager.currentText = dialogArray[0];
		dialogManager.nameLabel.Text = nameArray[0];
		dialogManager.dialogPanel.Visible = true;
	}

	public static void StartSaveDialog(string[] dialogArray, string[] nameArray)
	{
		dialogManager.InitializeDialogArrays(dialogArray, nameArray);
		dialogManager.saveDialog = true;

		dialogManager.dialogIndex = 0;
		dialogManager.dialogueText.Text = dialogManager.currentText = dialogArray[0];
		dialogManager.nameLabel.Text = nameArray[0];
		dialogManager.dialogPanel.Visible = true;
	}

	public void InitializeDialogArrays(string[] dialogArray, string[] nameArray)
	{
		for (int t = 0; t < dialogArray.Length; t++)
		{
			activeDialog.Add(dialogArray[t]);
			speakerNames.Add(nameArray[t]);
		}
	}

	private void ForceCloseSaveDialogue()
	{
		EndDialog();
	}
}
