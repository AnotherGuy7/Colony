using Godot;
using System;

public class Door : Area2D
{

	[Export]
	public Texture doorClosed;

	[Export]
	public Texture doorOpen;

	private Sprite doorSprite;

	public override void _Ready()
	{
		doorSprite = GetNode<Sprite>("DoorSprite");
	}

	private void OnOpenAreaBodyEntered(object body)
	{
		if (body == Player.player)
		{
			doorSprite.Texture = doorOpen;
		}
	}


	private void OnOpenAreaBodyExited(object body)
	{
		if (body == Player.player)
		{
			doorSprite.Texture = doorClosed;
		}
	}
}
