using Godot;
using System;

public class LivingTrunk : RigidBody2D
{
	private AnimatedSprite livingTrunkAnim;
	private Timer stunDurationTimer;
	private Timer moveRestTimer;
	private Timer moveDurationTimer;

	private const float moveSpeed = 0.4f;

	private int health = 5;
	private int flashTimer = 0;
	private string direction = "Front";
	private string restrictedDirection = "";
	private bool moving = false;
	private bool stunned = false;
	private bool alerted = false;
	private Vector2 knockbackVector;

	private Random rand = new Random();

	private readonly Color reddened = new Color(1f, 0.28f, 0.28f, 1f);
	private readonly Color normal = new Color(1f, 1f, 1f, 1f);

	public override void _Ready()
	{
		livingTrunkAnim = GetNode<AnimatedSprite>("LivingTrunkAnim");
		stunDurationTimer = GetNode<Timer>("StunDurationTimer");
		moveRestTimer = GetNode<Timer>("MoveRestTimer");
		moveDurationTimer = GetNode<Timer>("MoveDurationTimer");
	}

	public override void _Process(float delta)
	{
		if (stunned)
		{
			moving = false;
			//moveRestTimer.WaitTime = 0;
			//moveDurationTimer.WaitTime = 0;
			Flash(6);
		}
		else
		{
			Modulate = normal;
			knockbackVector = Vector2.Zero;
		}
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 velocity = Vector2.Zero;
		if (!alerted)
		{
			//Choosing a random direction upon being able to move
			if (moving && moveDurationTimer.TimeLeft == 0)
			{
				switch (rand.Next(0, 4))
				{
					case 0:
						direction = "Front";
						break;
					case 1:
						direction = "Back";
						break;
					case 2:
						direction = "Left";
						break;
					case 3:
						direction = "Right";
						break;
				}
				if (direction == restrictedDirection)
				{
					while (direction == restrictedDirection)
					{
						switch (rand.Next(0, 4))
						{
							case 0:
								direction = "Front";
								break;
							case 1:
								direction = "Back";
								break;
							case 2:
								direction = "Left";
								break;
							case 3:
								direction = "Right";
								break;
						}
					}
				}
				moveDurationTimer.Start(rand.Next(1, 3));
				moveRestTimer.Stop();
				restrictedDirection = "";
			}
			//Moving depending on direction
			if (moving)
			{
				if (direction == "Front")
				{
					velocity.y += moveSpeed;
				}
				if (direction == "Back")
				{
					velocity.y -= moveSpeed;
				}
				if (direction == "Left")
				{
					velocity.x -= moveSpeed;
				}
				if (direction == "Right")
				{
					velocity.x += moveSpeed;
				}
			}
		}
		else
		{
			Vector2 playerPos = Player.player.GlobalPosition;
			if (playerPos.y > GlobalPosition.y)
			{
				direction = "Front";
			}
			if (playerPos.y < GlobalPosition.y)
			{
				direction = "Back";
			}
			if (playerPos.x < GlobalPosition.x - 10f)
			{
				direction = "Left";
			}
			if (playerPos.x > GlobalPosition.x + 10f)
			{
				direction = "Right";
			}
			if (!stunned)
			{
				Vector2 newVelocity = playerPos - GlobalPosition;
				velocity = newVelocity.Normalized() * moveSpeed;
			}
			else
			{
				if (knockbackVector == Vector2.Zero)
					knockbackVector = playerPos - GlobalPosition;
				knockbackVector *= 0.4f;
				velocity = -knockbackVector;
			}
		}

		if (velocity == Vector2.Zero)
		{
			livingTrunkAnim.Play("Idle_" + direction);
		}
		else
		{
			livingTrunkAnim.Play("Walking_" + direction);
		}

		MoveLocalX(velocity.x);
		MoveLocalY(velocity.y);
	}

	private void StopMoving(int minimumTime = 1, int maximumTime = 2)
	{
		moving = false;
		moveRestTimer.Start(rand.Next(minimumTime, maximumTime + 1));
		moveDurationTimer.Stop();
	}
	
	//Player detection
	private void OnDetectionAreaBodyEntered(object body)
	{
		if (body == Player.player)
		{
			alerted = true;
		}
	}
	
	
	private void OnDetectionAreaBodyExited(object body)
	{
		if (body == Player.player)
		{
			alerted = false;
		}
	}

	//Move timers
	private void OnMoveRestTimerOut()
	{
		moving = true;
	}

	private void OnMoveDurationTimerOut()
	{
		StopMoving(2, 3);
	}

	//Attacked and stun stuff
	private void OnLivingTrunkBodyCollided(object body)
	{
		if (body == Player.player)
		{
			GameData.HurtPlayer(1, GlobalPosition, 6f);
		}
		StopMoving();
	}

	private void OnHitboxBodyEntered(object body)
	{
		if (body == Player.player)
		{
			GameData.HurtPlayer(1, GlobalPosition, 6f);
		}
		else
		{
			restrictedDirection = direction;
		}
		StopMoving();
	}

	private void OnHitboxAreaEntered(object area)
	{
		if (area == Player.playerSword)
		{
			health -= GameData.playerDamage;
			stunned = true;
			stunDurationTimer.Start();
			if (health <= 0)
			{
				GameData.SpawnDeathClouds(GlobalPosition);
				GameData.gameData.EmitSignal(nameof(GameData.UpdateQuestProgress), GetType().ToString());
				QueueFree();
			}
		}
	}

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
