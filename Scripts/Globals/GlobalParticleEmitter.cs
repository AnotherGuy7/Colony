using Godot;
using System;

public class GlobalParticleEmitter : Node2D
{
	public static Particles2D deathCloudEmitter;

	public override void _Ready()
	{
		deathCloudEmitter = GetNode<Particles2D>("DeathClouds");
	}

	public static void EmitDeathClouds(Vector2 pos, int amount)		//I'm not sure but doing this as a signal might require me to connect the signal from everywhere I want to emit it
	{
		deathCloudEmitter.GlobalPosition = pos;
		deathCloudEmitter.Amount = amount;
		deathCloudEmitter.Emitting = true;
	}
}
