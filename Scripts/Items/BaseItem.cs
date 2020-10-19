using Godot;
using System;

public class BaseItem : Area2D
{
	[Export]
	public int itemType;

	[Export]
	public int amount;

	private Sprite pickUpBox;
	private bool canBePickedUp = false;
	private int initializeTimer = 0;

	public override void _Ready()
	{
		pickUpBox = GetNode<Sprite>("PickUpBox");
	}

	public override void _Process(float delta)
	{
		if (initializeTimer > 0)
			initializeTimer--;

		if (canBePickedUp && Input.IsKeyPressed((int)KeyList.G) && initializeTimer <= 0)
		{
			Item.AddItemToInventory(Item.itemList[itemType], amount);
			QueueFree();
		}
	}


	private void OnBodyEntered(object body)
	{
		if (body == Player.player)
		{
			canBePickedUp = true;
			pickUpBox.Visible = true;
		}
	}


	private void OnBodyExited(object body)
	{
		if (body == Player.player)
		{
			canBePickedUp = false;
			pickUpBox.Visible = false;
		}
	}
}
