using Godot;
using System;

public class Apple : Area2D
{
	private Sprite pickUpBox;
	private bool canBePickedUp = false;

	public override void _Ready()
	{
		pickUpBox = GetNode<Sprite>("PickUpBox");
	}

	public override void _Process(float delta)
	{
		if (canBePickedUp && Input.IsKeyPressed((int)KeyList.G))
		{
			GameData.AddItemToInventory(Item.itemList[(int)Item.ItemTypes.Apple], 1);
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
