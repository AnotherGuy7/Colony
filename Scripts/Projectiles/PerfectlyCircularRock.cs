using Godot;
using System;
using System.Collections;

public class PerfectlyCircularRock : Area2D
{
	private Sprite rockSprite;

	public override void _Ready()
	{
		rockSprite = GetNode<Sprite>("PerfectlyCircularRock");
	}

	public override void _Process(float delta)
	{
		rockSprite.RotationDegrees += 1f;
	}

	private void OnBodyEntered(object body)
	{
		if (body == Player.player)
		{
			GameData.HurtPlayer(2);
		}
		QueueFree();
	}
}
