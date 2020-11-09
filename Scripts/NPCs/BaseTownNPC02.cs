using Godot;
using System;

public class BaseTownNPC02 : RigidBody2D
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
	public string NPCName = "";		//This is used for quest completion comparison


	//[Export]
	//public int questKey = 0;

	[Export]
	public string questName = "";

	[Export]
	public string questDescription = "";

	[Export]
	public int questType;

	[Export]
	public string targetNPCName;

	[Export]
	public string questsFullMessage = "";

	[Export]
	public int questObjectiveAmount = 0;

	[Export]
	public string[] questDoneDialog;

	[Export]
	public string[] questDoneNames;

	[Export]
	public int playerRewardMoney = 0;

	[Export]
	public int playerRewardItemType = 0;

	[Export]
	public int playerRewardItemStack = 0;

	private AnimatedSprite npcAnim;
	private Timer moveDurationTimer;
	private Timer moveRestTimer;

	private const float moveSpeed = 0.77f;

	private string direction = "Front";
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
			moveDurationTimer.Start(rand.Next(1, 3));
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
				moving = false;
				moveRestTimer.Start(rand.Next(2, 3));
				moveDurationTimer.Stop();
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
			bool questDone = false;
			for (int q = 0; q < GameData.activeQuests.Length; q++)
			{
				Quests quest = GameData.activeQuests[q];
				if (quest.askerName == NPCName && quest.progress == quest.maxProgress)
				{
					questDone = true;
					break;
				}
			}

			if (!questDone)
			{
				DialogueManager.StartDialogWithQuest(dialogue, speakerNames, questName, questDescription, questType, targetNPCName, NPCName, questsFullMessage, questObjectiveAmount);
				isBeingTalkedTo = true;
				GameData.isPlayerTalking = true;
			}
			else
			{
				DialogueManager.StartDialogWithReward(questDoneDialog, questDoneNames, playerRewardMoney, Item.itemList[playerRewardItemType], playerRewardItemStack);
				isBeingTalkedTo = true;
				GameData.isPlayerTalking = true;
			}
		}
	}

	//Move Timers
	private void OnMoveRestTimerOut()
	{
		moving = true;
	}

	private void OnMoveDurationTimerOut()
	{
		moving = false;
		moveRestTimer.Start(rand.Next(2, 3));
		moveDurationTimer.Stop();
	}

	private void OnBodyEntered(object body)
	{
		if (body.GetType().ToString() == "StaticBody2D")
		{
			moving = false;
			moveRestTimer.Start(rand.Next(2, 3));
			moveDurationTimer.Stop();
		}
	}

	private void OnStaticBodyEntered(int body_id, object body, int body_shape, int local_shape)
	{
		moving = false;
		moveRestTimer.Start(rand.Next(2, 3));
		moveDurationTimer.Stop();
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
