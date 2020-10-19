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

	public override void _Input(InputEvent @event)
	{
		if (canBeOpened)
		{
			if (Input.IsActionJustPressed("Continue"))
			{
				opened = true;
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
