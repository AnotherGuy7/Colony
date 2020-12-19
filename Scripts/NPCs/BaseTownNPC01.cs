using Godot;
using System;

public class BaseTownNPC01 : RigidBody2D
{
	[Export]
	public string[] dialogue = new string[4];

	[Export]
	public string[] speakerNames = new string[4];

	[Export]
	public bool anchored = false;

	[Export]
	public float anchoredDistance = 0f;

	[Export]
	public string anchoredDirectionToFace = "";

	private AnimatedSprite npcAnim;
	private Timer moveDurationTimer;
	private Timer moveRestTimer;

	private const float moveSpeed = 0.77f;

	private string direction = "Front";
	private string restrictedDirection = "";
	private bool canBeTalkedTo = false;
	private bool moving = false;
	private bool isBeingTalkedTo = false;
	private Vector2 savedStartPosition;     //this is what we use to measure the distance for anchors

	private Random rand = new Random();

	public override void _Ready()
	{
		npcAnim = GetNode<AnimatedSprite>("NPCAnim");
		moveDurationTimer = GetNode<Timer>("MoveDurationTimer");
		moveRestTimer = GetNode<Timer>("MoveRestTimer");
	}

	public override void _PhysicsProcess(float delta)
	{
		Vector2 velocity = Vector2.Zero;

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

		if (anchored)       //how it works is that if the NPC leaves distance it's just gonna stop until it picks a direction the goes back within range
		{
			Vector2 distanceFromAnchor = GlobalPosition - savedStartPosition;
			if (distanceFromAnchor.Length() > anchoredDistance)
			{
				StopMoving();
			}
			if (anchoredDirectionToFace != "")
			{
				direction = anchoredDirectionToFace;
			}
		}
		if (!isBeingTalkedTo)
		{
			if (velocity == Vector2.Zero)
			{
				npcAnim.Play("Idle_" + direction);
			}
			else
			{
				npcAnim.Play("Walking_" + direction);
			}

			MoveLocalX(velocity.x);
			MoveLocalY(velocity.y);
		}
		else
		{
			float angle = Mathf.Rad2Deg((GlobalPosition - Player.player.GlobalPosition).Angle()) + 180f;
			if (angle >= 135f && angle <= 225f)        //Left
			{
				direction = "Left";
			}
			if (angle >= 225f && angle <= 315f)        //Back
			{
				direction = "Back";
			}
			if (angle <= 45f || angle >= 315f)        //Right
			{
				direction = "Right";
			}
			if (angle >= 45f && angle <= 135f)        //Front
			{
				direction = "Front";
			}
			moveRestTimer.Stop();
			npcAnim.Play("Idle_" + direction);
			isBeingTalkedTo = GameData.isPlayerTalking;
		}
	}

	public override void _Process(float delta)
	{
		if (canBeTalkedTo && !isBeingTalkedTo && Input.IsActionJustPressed("Continue") && !GameData.isPlayerTalking)
		{
			DialogueManager.StartDialog(dialogue, speakerNames);
			isBeingTalkedTo = true;
		}
	}

	private void StopMoving(int minimumTime = 2, int maximumTime = 3)
	{
		moving = false;
		moveRestTimer.Start(rand.Next(minimumTime, maximumTime + 1));
		moveDurationTimer.Stop();
	}

	//Move Timers
	private void OnMoveRestTimerOut()
	{
		moving = true;
	}

	private void OnMoveDurationTimerOut()
	{
		StopMoving();
	}

	private void OnCollisionAreaBodyEntered(object body)
	{
		StopMoving();
		restrictedDirection = direction;
	}


	//Talk stuff
	private void OnTalkAreaEntered(object body)
	{
		if (body == Player.player)
		{
			canBeTalkedTo = true;
		}
	}


	private void OnTalkAreaExited(object body)
	{
		if (body == Player.player)
		{
			canBeTalkedTo = false;
		}
	}
}
