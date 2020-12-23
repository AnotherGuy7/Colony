using Godot;
using System;

public class Player : KinematicBody2D
{
	private const float moveSpeed = 45f;

	private AnimatedSprite playerAnim;
	private AnimatedSprite weaponAnim;
	private CollisionShape2D swordShape;
	private PackedScene arrow;
	private AnimationPlayer animPlayer;
	private AudioStreamPlayer swingSound;
	private Sprite itemObtainedTexture;

	private string direction = "Front";     //the way the player looks at the camera, NOT the direction he's going
	private bool shooting = false;
	private bool running = false;
	private int flashTimer = 0;
	private int hurtTimer = 0;
	private bool canMove = true;

	//State stuff
	private bool swinging = false;
	private bool eating = false;
	private bool itemObtained = false;

	public static Player player;
	public static Area2D playerSword;       //for easier player sword referencing
	public static Camera2D playerCam;
	public static bool moveDuringTransition = false;
	public static Vector2 transitionVelocityVector = Vector2.Zero;

	private Random rand = new Random();

	private readonly Color reddened = new Color(1f, 0.28f, 0.28f, 1f);
	private readonly Color normal = new Color(1f, 1f, 1f, 1f);

	public override void _Ready()
	{
		player = this;
		playerSword = GetNode<Area2D>("SwordHitbox");
		playerAnim = GetNode<AnimatedSprite>("PlayerAnim");
		weaponAnim = GetNode<AnimatedSprite>("WeaponAnim");
		swordShape = GetNode<CollisionShape2D>("SwordHitbox/SwordShape");
		swingSound = GetNode<AudioStreamPlayer>("SwingSound");
		itemObtainedTexture = GetNode<Sprite>("ItemObtainedTexture");
		animPlayer = GetNode<AnimationPlayer>("AnimPlayer");
		playerCam = GetNode<Camera2D>("PlayerCam");

		arrow = GD.Load<PackedScene>("res://Scenes/Environment/Projectiles/Arrow.tscn");

		GameData.gameData.Connect(nameof(GameData.SwitchedMaps), this, nameof(SpawnedInMap));
		GameData.gameData.EmitSignal(nameof(GameData.UpdateInventorySlotDrawings));
	}

	public override void _Input(InputEvent @event)
	{
		if (Input.IsActionJustPressed("attack"))
		{
			HandleItemUsages(GameData.playerInventory[GameData.selectedInventorySlot]);
		}
		if (Input.IsKeyPressed((int)KeyList.Shift))
		{
			running = true;
		}
	}

	public override void _Process(float delta)
	{
		swordShape.Disabled = !swinging;
		canMove = !GameData.isPlayerTalking && !GameData.playerDead && !itemObtained && !eating;

		HandleHitboxRotationAndSize();

		weaponAnim.Frame = playerAnim.Frame;

		if (GameData.playerHealth <= 0)
		{
			animPlayer.Play("DeathAnim");
		}

		//Post-Stun stuff
		if (!GameData.playerHurt && Modulate == reddened)
		{
			Modulate = normal;
		}

		if (!Input.IsActionPressed("attack") && shooting)
		{
			Area2D arrowInstance = arrow.Instance() as Area2D;
			Vector2 direction = GetGlobalMousePosition() - GlobalPosition;
			arrowInstance.Set("velocity", direction.Normalized() * 2f);
			arrowInstance.Set("rotation", Mathf.Rad2Deg(direction.Angle()) + 90f);
			GameData.mapYSort.AddChild(arrowInstance);
			arrowInstance.GlobalPosition = GlobalPosition;
			shooting = false;
		}

		if (Input.IsKeyPressed((int)KeyList.Equal))
			if (playerCam.Zoom.x > 0.5f)
				playerCam.Zoom -= new Vector2(0.05f, 0.05f);

		if (Input.IsKeyPressed((int)KeyList.Minus))
			if (playerCam.Zoom.x < 1.5f)
				playerCam.Zoom += new Vector2(0.05f, 0.05f);
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 velocity = Vector2.Zero;

		if (canMove && !GameData.transitioning)
		{
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
		}

		if (!swinging && !eating && !itemObtained)
		{
			if (velocity == Vector2.Zero)
			{
				playerAnim.Play("Idle_" + direction);
			}
			else
			{
				if (running)
					playerAnim.Play("Running_" + direction);
				else
					playerAnim.Play("Walking_" + direction);
			}
		}
		else
		{
			velocity = Vector2.Zero;
			if (swinging)
			{
				playerAnim.Play("Swinging_" + direction);
			}
			else if (eating)
			{
				direction = "Front";
				playerAnim.Play("Eating");
			}
			else if (itemObtained)
			{
				direction = "Front";
				playerAnim.Play("ItemObtained");
			}
		}

		//Hurt movement
		if (running)
		{
			velocity *= 2f;
		}
		if (shooting)
		{
			velocity /= 2f;
		}
		if (velocity == Vector2.Zero)
		{
			running = false;
		}
		if (GameData.playerHurt)
		{
			hurtTimer++;
			velocity = GameData.inflictedKnockbackVector;
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
		if (GameData.transitioning)
        {
			if (moveDuringTransition && transitionVelocityVector == Vector2.Zero)
            {
				transitionVelocityVector = velocity;
            }
			velocity = transitionVelocityVector;
        }

		if (canMove)
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

		if (GameData.playerSavedPosition != Vector2.Zero)
		{
			GlobalPosition = GameData.playerSavedPosition;
			GameData.playerSavedPosition = Vector2.Zero;
		}

		direction = passedDirection;
	}

	private void OnPlayerAnimAnimationDone()
	{
		swinging = false;
		weaponAnim.Visible = false;
		weaponAnim.Stop();
		if (playerAnim.Animation == "Eating")
		{
			canMove = true;
			eating = false;
			playerAnim.Play("Idle_" + direction);
		}
	}

	private void OnAnimPlayerAnimationFinished(String anim_name)
	{
		if (anim_name == "DeathAnim")
		{
			SceneSwitcher.sceneSwitcher.GotoScene("DeathScreen", 0, "Back");
		}
	}


	//Methods
	public static void PlayItemObtainedAnimation(int itemType)
	{
		player.itemObtained = true;
		player.itemObtainedTexture.Visible = true;
		player.itemObtainedTexture.Texture = Item.itemsDict[itemType].sprite;
		player.animPlayer.Play("Item Obtained");
		player.canMove = false;
	}

	public static void StopItemObtainedAnimation()
	{
		player.itemObtained = false;
		player.itemObtainedTexture.Visible = false;
		player.animPlayer.Stop();
		player.canMove = true;
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

	private void HandleItemUsages(Item item, int consumeAmount = 1)
	{
		switch (item.useType)
		{
			case Item.Weapon:
				Attack(item.type);
				weaponAnim.Visible = true;
				swingSound.Stream = item.useSound;
				swingSound.Play();
				break;
			case Item.Healing:
				eating = true;
				canMove = false;
				GameData.playerHealth += item.healAmount;
				Item.ConsumeItem(item, consumeAmount);
				break;
			case Item.Equip:
				break;
		}
	}

	private void Attack(int itemType)
	{
		switch (itemType)
		{
			case Item.DullDagger:
				swinging = true;
				weaponAnim.Play("DullDagger_" + direction);
				weaponAnim.Stop();
				break;
			case Item.Rapier:
				swinging = true;
				weaponAnim.Play("Rapier_" + direction);
				weaponAnim.Stop();
				break;
			case Item.Bow:
				shooting = true;
				break;
		}
	}
}
