using Godot;
using System;

public class BaseMerchant : RigidBody2D
{
	[Export]
	public string starterText;

	[Export]
	public string speakerName;

	[Export]
	public string[] itemsToSell = new string[5];

	private bool canBeTalkedTo = false;
	private int[] itemsToSellTypes = new int[5];

	public override void _Process(float delta)
	{
		if (canBeTalkedTo && Input.IsActionJustPressed("Continue") && !GameData.isPlayerTalking)
		{
			for (int i = 0; i < itemsToSell.Length; i++)
			{
				itemsToSellTypes[i] = Item.GetItemType(itemsToSell[i]);
			}
			ShopMenu.StartShop(starterText, speakerName, itemsToSellTypes);
			GameData.isPlayerTalking = true;
		}
	}

	//Talk stuff
	private void OnTalkAreaEntered(object body)
	{
		if (body == Player.player)
		{
			canBeTalkedTo = true;
		}
	}


	private void OnTalkAreaExited(object body)
	{
		if (body == Player.player)
		{
			canBeTalkedTo = false;
		}
	}
}
