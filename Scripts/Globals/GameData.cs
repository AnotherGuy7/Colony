using Godot;

public class GameData : Node2D
{
	[Signal]
	public delegate void SwitchedMaps(int positionToSpawnAt, string direction = "Front");		//this is sent after a map change

	[Signal]
	public delegate void UpdateInventorySlotDrawings();       //this is sent after the player receives an item, or uses one

	[Signal]
	public delegate void UpdateQuestProgress(string enemyName);       //this is sent after a map change

	[Signal]
	public delegate void SavedGame();

	public static GameData gameData;
	public static YSort mapYSort;

	//Player stats
	public static int playerMaxHealth = 4;
	public static int playerHealth = 8;
	public static int playerDamage = 1;
	public static int playerCurrency = 100;
	public static string playerLocation = "";
	public static bool playerDead = false;
	//public static Item[] playerInventory = new Item[5] { Item.itemDict[(int)Item.ItemTypes.Air], Item.itemDict[(int)Item.ItemTypes.Air], Item.itemDict[(int)Item.ItemTypes.Air], Item.itemDict[(int)Item.ItemTypes.Air], Item.itemDict[(int)Item.ItemTypes.Air]};
	public static Item[] playerInventory = new Item[5];
	public static Quests[] activeQuests = new Quests[3];

	//Player action bools
	public static bool playerHurt = false;      //I could also emit a signal... but this is easier and a signal would require me to check anyway
	public static bool isPlayerTalking = false;

	//Misc stuff
	public const int MaxSaveSlots = 10;
	public static Vector2 inflictedKnockbackVector = Vector2.Zero;
	public static bool inventoryFull = false;
	public static int selectedInventorySlot = 0;
	public static int latestOpenSlot = 1;
	public static Vector2 playerSavedPosition;
	public static int currentPlayerSaveIndex = 0;

	public override void _Ready()
	{
		gameData = this;
		Connect(nameof(UpdateQuestProgress), this, nameof(UpdateQuestProgressMethod));
		for (int i = 0; i < playerInventory.Length; i++)
		{
			if (i == 0)
				playerInventory[i] = Item.itemList[(int)Item.ItemTypes.Sword];
			else
				playerInventory[i] = Item.itemList[(int)Item.ItemTypes.Air];
		}

		for (int q = 0; q < activeQuests.Length; q++)		//This is so that upon loading, the array has object values while will be modified on save
		{
			activeQuests[q] = Quests.emptyQuest;
		}
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton scroll)
		{
			if (scroll.IsPressed())
			{
				if (scroll.ButtonIndex == (int)ButtonList.WheelUp)
					selectedInventorySlot--;

				if (scroll.ButtonIndex == (int)ButtonList.WheelDown)
					selectedInventorySlot++;

				if (selectedInventorySlot >= playerInventory.Length)
					selectedInventorySlot = 0;
				if (selectedInventorySlot < 0)
					selectedInventorySlot = playerInventory.Length - 1;

				gameData.EmitSignal(nameof(UpdateInventorySlotDrawings));
			}
		}
	}

	private void UpdateQuestProgressMethod(string name)
	{
		for (int q = 0; q < activeQuests.Length; q++)
		{
			if (activeQuests[q].targetNPCName == name)
			{
				activeQuests[q].progress += 1;
			}
		}
	}

	//Methods that can be used anywhere
	public static void HurtPlayer(int damage, Vector2 enemyPos, float knockBackStrength = 1f)
	{
		playerHealth -= damage;
		playerHurt = true;
		Vector2 direction = Player.player.GlobalPosition - enemyPos;
		inflictedKnockbackVector = direction.Normalized() * knockBackStrength;
	}

	public static void SpawnDeathClouds(Vector2 pos)
	{
		AnimatedSprite clouds = PackedScenes.scenesDict["DeathClouds"].Instance() as AnimatedSprite;
		mapYSort.AddChild(clouds);
		clouds.GlobalPosition = pos;
		clouds.Play("default");
	}
}
