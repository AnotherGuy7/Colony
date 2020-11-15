using Godot;
using System;

public class Chests : Area2D
{
	[Export]
	public int itemType = -1;

	[Export]
	public Texture openedChest;

	private bool opened = false;
	private bool canBeOpened = false;
	private Sprite chestSprite;
	private string[] dialogue = new string[1];
	private string[] names = new string[1];

	public override void _Ready()
	{
		chestSprite = GetNode<Sprite>("ChestSprite");
	}

	public override void _Input(InputEvent @event)
	{
		if (canBeOpened)
		{
			if (Input.IsActionJustPressed("Continue"))
			{
				opened = true;
				dialogue[0] = "You obtained a " + Item.itemList[itemType].name + "!";
				names[0] = "";
				DialogueManager.StartDialogForItem(dialogue, names);
				Player.PlayItemObtainedAnimation(itemType);
				chestSprite.Texture = openedChest;
				canBeOpened = false;
			}
		}
	}

	private void OnChestBodyEntered(object body)
	{
		if (body == Player.player && !opened)
		{
			canBeOpened = true;
		}
	}


	private void OnChestBodyExited(object body)
	{
		if (body == Player.player)
		{
			canBeOpened = false;
		}
	}
}
