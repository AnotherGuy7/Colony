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
	
	private string currentText = "";
	private string currentName = "";
	private int dialogIndex = -1;
	private float textPercentage = 0f;

	public override void _Ready()
	{
		dialogManager = this;
		dialogueText = GetNode<Label>("Layer2/DialogPanel/DialogueText");
		nameLabel = GetNode<Label>("Layer2/DialogPanel/NameLabel");
		dialogPanel = GetNode<Panel>("Layer2/DialogPanel");
		talkSound = GetNode<AudioStreamPlayer>("TalkSound");
	}

	public override void _Process(float delta)
	{
		if (GameData.isPlayerTalking)
		{
			if (textPercentage < 1f)
			{
				textPercentage += 0.005f;
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
						if (dialogIndex > activeDialog.Count - 1)
						{
							dialogIndex = -1;
							activeDialog.Clear();
							dialogPanel.Visible = false;
							GameData.isPlayerTalking = false;
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

	public static void StartDialog(string[] dialogArray, string[] nameArray)
	{
		for (int t = 0; t < dialogArray.Length; t++)       //I'm not sure if there's a better way to do this?
		{
			activeDialog.Add(dialogArray[t]);
			speakerNames.Add(nameArray[t]);
		}
		dialogManager.dialogIndex = 0;
		dialogManager.dialogueText.Text = dialogArray[0];
		dialogManager.nameLabel.Text = nameArray[0];
		dialogManager.dialogPanel.Visible = true;
	}
}
