using Godot;
using System;

public class Player : KinematicBody2D
{
	private const float moveSpeed = 45f;

	private AnimatedSprite playerAnim;
	private AnimatedSprite weaponAnim;
	private CollisionShape2D swordShape;
	private PackedScene arrow;
	private AudioStreamPlayer swingSound;
	private AudioStreamPlayer dirtStepSound;
	private Timer stepTimer;

	private string direction = "Front";     //the way the player looks at the camera, NOT the direction he's going
	private bool swinging = false;
	private bool shooting = false;
	private bool running = false;
	private int flashTimer = 0;
	private int hurtTimer = 0;

	public static Player player;
	public static Area2D playerSword;       //for easier player sword referencing
	public static Camera2D playerCam;

	private Random rand = new Random();

	private readonly Color reddened = new Color(1f, 0.28f, 0.28f, 1f);
	private readonly Color normal = new Color(1f, 1f, 1f, 1f);

	private AudioStream dirtStep1;
	private AudioStream dirtStep2;
	private AudioStream dirtStep3;
	private AudioStream dirtStep4;

	public override void _Ready()
	{
		player = this;
		playerSword = GetNode<Area2D>("SwordHitbox");
		playerAnim = GetNode<AnimatedSprite>("PlayerAnim");
		weaponAnim = GetNode<AnimatedSprite>("WeaponAnim");
		swordShape = GetNode<CollisionShape2D>("SwordHitbox/SwordShape");
		swingSound = GetNode<AudioStreamPlayer>("SwingSound");
		dirtStepSound = GetNode<AudioStreamPlayer>("DirtStepSound");
		stepTimer = GetNode<Timer>("StepTimer");
		playerCam = GetNode<Camera2D>("PlayerCam");

		arrow = GD.Load<PackedScene>("res://Scenes/Environment/Projectiles/Arrow.tscn");

		dirtStep1 = GD.Load("res://Sounds/SoundEffects/DirtStep1.wav") as AudioStream;
		dirtStep2 = GD.Load("res://Sounds/SoundEffects/DirtStep2.wav") as AudioStream;
		dirtStep3 = GD.Load("res://Sounds/SoundEffects/DirtStep3.wav") as AudioStream;
		dirtStep4 = GD.Load("res://Sounds/SoundEffects/DirtStep4.wav") as AudioStream;

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

		HandleHitboxRotationAndSize();

		weaponAnim.Frame = playerAnim.Frame;

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
			stepTimer.Stop();
		}
		else
		{
			if (stepTimer.TimeLeft == 0)
			{
				stepTimer.Start();
			}
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
	}

	private void OnStepTimerOut()
	{
		switch (rand.Next(1, 5))
		{
			case 1:
				dirtStepSound.Stream = dirtStep1;
				break;
			case 2:
				dirtStepSound.Stream = dirtStep2;
				break;
			case 3:
				dirtStepSound.Stream = dirtStep3;
				break;
			case 4:
				dirtStepSound.Stream = dirtStep4;
				break;
		}
		dirtStepSound.Play();
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
				GameData.playerHealth += item.healAmount;
				GameData.ConsumeItem(item, consumeAmount);
				break;
			case Item.Equip:
				break;
		}
	}

	private void Attack(int itemType)
	{
		switch (itemType)
		{
			case (int)Item.ItemTypes.Sword:
				swinging = true;
				weaponAnim.Play("Sword_" + direction);
				weaponAnim.Stop();
				break;
			case (int)Item.ItemTypes.Rapier:
				swinging = true;
				weaponAnim.Play("Rapier_" + direction);
				weaponAnim.Stop();
				break;
			case (int)Item.ItemTypes.Bow:
				shooting = true;
				break;
		}
	}
}
