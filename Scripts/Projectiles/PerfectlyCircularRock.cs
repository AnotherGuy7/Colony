using Godot;
using System;
using System.Collections;

public class PerfectlyCircularRock : Area2D
{
	private Sprite rockSprite;
	public Vector2 velocity;

	public override void _Ready()
	{
		rockSprite = GetNode<Sprite>("PerfectlyCircularRock");
	}

	public override void _Process(float delta)
	{
		rockSprite.RotationDegrees += 5f;
	}

	public override void _PhysicsProcess(float delta)
	{
		MoveLocalX(velocity.x);
		MoveLocalY(velocity.y);
	}

	private void OnBodyEntered(object body)
	{
		if (body == Player.player)
		{
			GameData.HurtPlayer(2);
		}
		if (body.GetType().ToString() != "Rocklin")
			QueueFree();
	}
}
