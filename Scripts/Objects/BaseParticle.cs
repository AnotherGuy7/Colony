using Godot;
using System;

public class BaseParticle : AnimatedSprite
{
	private void OnTimeLeftOut()
	{
		QueueFree();
		GD.Print("Ded");
	}
}
