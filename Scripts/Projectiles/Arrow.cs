using Godot;
using System;

public class Arrow : Area2D
{
	private Vector2 velocity;
	private float rotation;
	private Sprite arrowSprite;

	public override void _Ready()
	{
		arrowSprite = GetNode<Sprite>("ArrowSprite");
	}

	public override void _PhysicsProcess(float delta)
	{
		arrowSprite.RotationDegrees = rotation;

		MoveLocalX(velocity.x);
		MoveLocalY(velocity.y);
	}

	private void OnBodyEntered(object body)
	{
		if (body != Player.player)
			QueueFree();
	}

	private void OnTimeleftOut()
	{
		QueueFree();
	}
}
