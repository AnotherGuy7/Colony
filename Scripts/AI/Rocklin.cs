using Godot;
using System;

public class Rocklin : RigidBody2D
{
	[Export]
	public PackedScene perfectlyCircularRock;

	private AnimatedSprite rocklinAnim;
	private Timer searchTimer;
	private Timer searchRestTimer;
	private Timer attackTimer;
	private Timer stunDurationTimer;
	private Timer meleeAttackTimer;
	private Area2D detectionArea;

	private const float shootSpeed = 1.5f;

	private int health = 6;
	private int flashTimer = 0;
	private int playerDetectionTimer = 0;

	private bool searching = false;
	private bool alerted = false;
	private bool stunned = false;

	private readonly Color reddened = new Color(1f, 0.28f, 0.28f, 1f);
	private readonly Color normal = new Color(1f, 1f, 1f, 1f);

	public override void _Ready()
	{
		rocklinAnim = GetNode<AnimatedSprite>("RocklinAnim");
		searchTimer = GetNode<Timer>("SearchTimer");
		searchRestTimer = GetNode<Timer>("SearchRestTimer");
		attackTimer = GetNode<Timer>("AttackTimer");
		stunDurationTimer = GetNode<Timer>("StunDurationTimer");
		meleeAttackTimer = GetNode<Timer>("MeleeAttackTimer");
		detectionArea = GetNode<Area2D>("DetectionRange");
	}

	public override void _Process(float delta)
	{
		if (playerDetectionTimer > 0)
		{
			playerDetectionTimer--;
		}

		if (searching)
		{
			if (playerDetectionTimer <= 0)
			{
				foreach (object body in detectionArea.GetOverlappingBodies())
				{
					if (body == Player.player)
					{
						alerted = true;
					}
				}
				playerDetectionTimer += 60;
			}
			rocklinAnim.Play("Searching");
		}
		else
		{
			if (!alerted)
			{
				rocklinAnim.Play("Idle");
				if (searchRestTimer.IsStopped())
				{
					searchRestTimer.Start();
				}
				if (!attackTimer.IsStopped())
				{
					attackTimer.Stop();
				}
			}
			else
			{
				rocklinAnim.Play("Alerted");
			}
		}

		if (alerted)
		{
			searchTimer.Stop();
			searchRestTimer.Stop();
			searching = false;
			if (attackTimer.IsStopped())
			{
				attackTimer.Start();
			}	
		}

		if (stunned)
		{
			Flash(6);
		}
		else
		{
			Modulate = normal;
		}

		if (health <= 0)
		{
			QueueFree();
		}
	}

	private void OnAttackTimerOut()
	{
		Area2D rockProjectile = (Area2D)perfectlyCircularRock.Instance();
		GameData.mapYSort.AddChild(rockProjectile);
		rockProjectile.GlobalPosition = GlobalPosition;
		Vector2 directionToPlayer = Player.player.GlobalPosition - GlobalPosition;
		rockProjectile.Set("velocity", directionToPlayer.Normalized() * shootSpeed);
	}

	//Detection stuff
	private void OnSearchTimerOut()
	{
		searching = false;
		searchRestTimer.Start();
		searchTimer.Stop();
	}

	private void OnSearchRestTimerOut()
	{
		searching = true;
		searchTimer.Start();
		searchRestTimer.Stop();
	}

	private void OnDetectionRangeBodyEntered(object body)
	{
		if (body == Player.player && searching)
		{
			alerted = true;
		}
	}

	private void OnDetectionRangeBodyExited(object body)
	{
		if (body == Player.player)
		{
			alerted = false;
		}
	}

	//Damage dealing stuff
	private void OnHitboxAreaEntered(object area)
	{
		if (area == Player.playerSword)
		{
			health -= GameData.playerDamage;
			stunned = true;
			stunDurationTimer.Start();
		}
	}

	//Hurting the player if the player walks too close
	private void OnHitboxBodyEntered(object body)
	{
		if (body == Player.player && meleeAttackTimer.TimeLeft == 0 && alerted)
		{
			GameData.HurtPlayer(1);
			meleeAttackTimer.Start();
		}
	}

	//Stun stuff
	private void OnStunDurationTimerOut()
	{
		stunned = false;
	}

	private void Flash(int eachFlashDuration)
	{
		flashTimer++;
		if (flashTimer >= eachFlashDuration)
		{
			if (Modulate == normal)
			{
				Modulate = reddened;
			}
			else
			{
				Modulate = normal;
			}
			flashTimer = 0;
		}
	}
}
