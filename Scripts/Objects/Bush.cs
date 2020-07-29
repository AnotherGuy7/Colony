using Godot;
using System;

public class Bush : Area2D
{
	private Sprite bushSprite;
	private CollisionShape2D bushShape;
	private Timer deathTimer;
	private Particles2D leafParticles;
	
	public override void _Ready()
	{
		bushSprite = GetNode<Sprite>("BushSprite");
		bushShape = GetNode<CollisionShape2D>("BushBody/BushBodyShape");
		deathTimer = GetNode<Timer>("DeathTimer");
		leafParticles = GetNode<Particles2D>("LeafParticles");
	}
	
	private void OnBushAreaEntered(object area)
	{
		if (area == Player.playerSword)
		{
			CallDeferred(nameof(MakeBushDisappear));
		}
	}

	private void MakeBushDisappear()
	{
		bushSprite.Visible = false;
		bushShape.Disabled = true;
		deathTimer.Start();
		leafParticles.Emitting = true;
	}

	private void OnDeathTimerOut()
	{
		QueueFree();
	}
}
