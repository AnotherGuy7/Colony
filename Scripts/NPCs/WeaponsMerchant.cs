using Godot;
using System;

public class WeaponsMerchant : RigidBody2D
{
	[Export]
	public string starterText;

	[Export]
	public string speakerName;

	[Export]
	public int[] itemsToSell = new int[5];

	private bool canBeTalkedTo = false;

	public override void _Process(float delta)
	{
		if (canBeTalkedTo && Input.IsActionJustPressed("Continue") && !GameData.isPlayerTalking)
		{
			ShopMenu.StartShop(starterText, speakerName, itemsToSell);
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
