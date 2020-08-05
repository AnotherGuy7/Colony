using Godot;
using System;

public class Player : KinematicBody2D
{
	private const float moveSpeed = 45f;

	private AnimatedSprite playerAnim;
	private CollisionShape2D swordShape;

	private string direction = "Front";     //the way the player looks at the camera, NOT the direction he's going
	private bool swinging = false;

	public static Player player;
	public static Area2D playerSword;       //for easier player sword referencing
	public static Camera2D playerCam;

	public override void _Ready()
	{
		player = this;
		playerSword = GetNode<Area2D>("SwordHitbox");
		playerAnim = GetNode<AnimatedSprite>("PlayerAnim");
		swordShape = GetNode<CollisionShape2D>("SwordHitbox/SwordShape");
		playerCam = GetNode<Camera2D>("PlayerCam");
		//Connect(nameof(GameData.SwitchedMaps), this, nameof(SpawnedInMap));
		GameData.gameData.Connect(nameof(GameData.SwitchedMaps), this, nameof(SpawnedInMap));
	}

	public override void _Process(float delta)
	{
		swordShape.Disabled = !swinging;
		
		//Sword hitbox rotation
		if (direction == "Front")
		{
			playerSword.RotationDegrees = 0f;
		}
		if (direction == "Back")
		{
			playerSword.RotationDegrees = 180f;
		}
		if (direction == "Left")
		{
			playerSword.RotationDegrees = 90f;
		}
		if (direction == "Right")
		{
			playerSword.RotationDegrees = 270f;
		}

		//Hurt stun

	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 velocity = Vector2.Zero;

		if (Input.IsActionPressed("move_front"))
		{
			direction = "Front";
			velocity.y += moveSpeed;
		}
		if (Input.IsActionPressed("move_back"))
		{
			direction = "Back";
			velocity.y -= moveSpeed;
		}
		if (Input.IsActionPressed("move_left"))
		{
			direction = "Left";
			velocity.x -= moveSpeed;
		}
		if (Input.IsActionPressed("move_right"))
		{
			direction = "Right";
			velocity.x += moveSpeed;
		}
		if (Input.IsActionJustPressed("attack"))
		{
			swinging = true;
			velocity = Vector2.Zero;
		}

		if (!swinging)
		{
			if (velocity == Vector2.Zero)
			{
				playerAnim.Play("Idle_" + direction);
			}
			else
			{
				playerAnim.Play("Walking_" + direction);
			}
		}
		else
		{
			velocity = Vector2.Zero;
			playerAnim.Play("Swinging_" + direction);
		}

		//Hurt movement
		if (GameData.playerHurt)
		{
			velocity *= -2f;
			GameData.playerHurt = false;
		}
		if (GameData.isPlayerTalking)
		{
			velocity = Vector2.Zero;
		}

		MoveAndSlide(velocity);
	}

	private void OnPlayerAnimAnimationDone()
	{
		swinging = false;
	}

	private void SpawnedInMap(int positionToSpawnAt, string passedDirection)
	{
		foreach (object allChildren in GetTree().CurrentScene.GetChildren())
		{
			if (allChildren.GetType().ToString() == "Godot.Position2D")
			{
				Position2D potentialSpawnPoint = allChildren as Position2D;
				if (potentialSpawnPoint.Name.Contains("SpawnPos" + positionToSpawnAt))
				{
					GlobalPosition = potentialSpawnPoint.GlobalPosition;
				}
			}
		}
		direction = passedDirection;
	}
}
