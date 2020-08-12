using Godot;
using System;

public class Player : KinematicBody2D
{
	private const float moveSpeed = 45f;

	private AnimatedSprite playerAnim;
	private CollisionShape2D swordShape;

	private string direction = "Front";     //the way the player looks at the camera, NOT the direction he's going
	private bool swinging = false;
	private int flashTimer = 0;
	private int hurtTimer = 0;

	public static Player player;
	public static Area2D playerSword;       //for easier player sword referencing
	public static Camera2D playerCam;

	private readonly Color reddened = new Color(1f, 0.28f, 0.28f, 1f);
	private readonly Color normal = new Color(1f, 1f, 1f, 1f);

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

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("attack"))
		{
			swinging = true;
		}
	}

	public override void _Process(float delta)
	{
		swordShape.Disabled = !swinging;

		HandleHitboxRotationAndSize();

		//Post-Stun stuff
		if (!GameData.playerHurt && Modulate == reddened)
		{
			Modulate = normal;
		}

		if (Input.IsKeyPressed((int)KeyList.V))
		{
			for (int i = 0; i < GameData.playerInventory.Length; i++)
			{
				Item item = GameData.playerInventory[i];
				GD.Print(item.name);
			}
		}
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
			hurtTimer++;
			velocity *= -GameData.inflictedKnockback * 7f;
			Flash(6);
			if (hurtTimer >= 30)
			{
				GameData.playerHurt = false;
				hurtTimer = 0;
			}
		}
		if (GameData.isPlayerTalking)
		{
			velocity = Vector2.Zero;
		}

		MoveAndSlide(velocity);
	}

	//Signal stuff
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

	private void OnPlayerAnimAnimationDone()
	{
		swinging = false;
	}

	//Methods
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

	private void HandleHitboxRotationAndSize()
	{
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
	}

	private void HandleItemUsages(Item item, int consumeAmount = 0)
	{
		switch (item.useType)
		{
			case Item.Weapon:
				break;
			case Item.Healing:
				GameData.playerHealth += item.healAmount;
				item.stack -= consumeAmount;
				break;
			case Item.Equip:
				break;
		}
	}
}
