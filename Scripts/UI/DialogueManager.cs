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
	private float textPercentage = 0f;

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
			if (textPercentage < 1f)
			{
				textPercentage += 0.005f;
				//talkSound.PitchScale = rand.Next(97, 103) / 100f;
				//talkSound.Play();
			}
			if (Input.IsActionJustPressed("Continue"))
			{
				if (textPercentage < 1f)
				{
					textPercentage = 1f;
				}
				else
				{
					if (dialogIndex < activeDialog.Count)
					{
						dialogIndex++;
						if (dialogIndex > activeDialog.Count)
						{
							if (saveDialog)		//If it's the save prompt then just undo the addition, make the save panel show up, and stop updating text
							{
								//dialogIndex--;
								GD.Print("Run");
								return;
							}
							UI.ui.savePanel.Visible = true;
							EndDialog();
							return;
						}
						currentText = activeDialog[dialogIndex];
						dialogueText.Text = currentText;
						currentName = speakerNames[dialogIndex];
						nameLabel.Text = currentName;
						textPercentage = 0;
					}
				}
			}
			dialogueText.PercentVisible = textPercentage;
		}
	}

	private void EndDialog()
	{
		dialogIndex = -1;
		activeDialog.Clear();
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
		dialogManager.dialogueText.Text = dialogArray[0];
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

	public static void StartDialogWithQuest(string[] dialogArray, string[] nameArray, string questName, string questDesc, int questType, string targetNPCName, string questsFullMessage, int questAmount)
	{
		dialogManager.InitializeDialogArrays(dialogArray, nameArray);

		Quests newQuest = new Quests();
		newQuest.questName = questName;
		newQuest.questDescription = questDesc;
		newQuest.questType = questType;
		newQuest.progress = 0;
		newQuest.maxProgress = questAmount;
		newQuest.targetNPCName = targetNPCName;

		if (!Quests.AddQuest(newQuest, questAmount))       //tries to add the quest, and if it's full it's gonna add the quest failed line to the dialog
		{
			activeDialog.Add(questsFullMessage);
			speakerNames.Add(nameArray[0]);
		}

		dialogManager.dialogIndex = 0;
		dialogManager.dialogueText.Text = dialogArray[0];
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
		dialogManager.dialogueText.Text = dialogArray[0];
		dialogManager.nameLabel.Text = nameArray[0];
		dialogManager.dialogPanel.Visible = true;
	}

	public static void StartSaveDialog(string[] dialogArray, string[] nameArray)
	{
		dialogManager.InitializeDialogArrays(dialogArray, nameArray);
		dialogManager.saveDialog = true;
		UI.ui.savePanel.Visible = true;

		dialogManager.dialogIndex = 0;
		dialogManager.dialogueText.Text = dialogArray[0];
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
