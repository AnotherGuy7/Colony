using Godot;
using System.Runtime.CompilerServices;

public class GameData : Node2D
{
	[Signal]
	public delegate void SwitchedMaps(int positionToSpawnAt, string direction = "Front");		//this is sent after a map change

	[Signal]
	public delegate void UpdateInventorySlotDrawings();       //this is sent after the player receives an item, or uses one

	[Signal]
	public delegate void UpdateQuestProgress(string enemyName);       //this is sent after a map change

	public static GameData gameData;
	public static YSort mapYSort;

	//Player stats
	public static int playerMaxHealth = 4;
	public static int playerHealth = 8;
	public static int playerDamage = 1;
	public static int playerCurrency = 100;
	//public static Item[] playerInventory = new Item[5] { Item.itemDict[(int)Item.ItemTypes.Air], Item.itemDict[(int)Item.ItemTypes.Air], Item.itemDict[(int)Item.ItemTypes.Air], Item.itemDict[(int)Item.ItemTypes.Air], Item.itemDict[(int)Item.ItemTypes.Air]};
	public static Item[] playerInventory = new Item[5];
	public static Quests[] activeQuests = new Quests[3];

	//Player action bools
	public static bool playerHurt = false;      //I could also emit a signal... but this is easier and a signal would require me to check anyway
	public static bool isPlayerTalking = false;

	//Misc stuff
	public static float inflictedKnockback = 1f;
	public static bool inventoryFull = false;
	public static int selectedInventorySlot = 0;
	public static int latestOpenSlot = 1;

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
		activeQuests[0] = Quests.questsDict[0];
		activeQuests[1] = Quests.questsDict[-1];
		activeQuests[2] = Quests.questsDict[-1];
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
				activeQuests[q].progress -= 1;
			}
			if (activeQuests[q].progress < 0)
			{
				activeQuests[q].questDescription = "This quest is done. Go talk to " + activeQuests[q].askerName + " for your reward.";
			}
		}
	}

	//Methods that can be used anywhere
	public static void HurtPlayer(int damage, float knockBack = 1f)
	{
		playerHealth -= damage;
		playerHurt = true;
		inflictedKnockback = knockBack;
	}

	public static void AddItemToInventory(Item itemToGive, int stack)
	{
		Item selectedItem = playerInventory[selectedInventorySlot];
		bool itemPlaced = false;

		//First, we check if a stack of the item exists
		for (int i = 0; i < 5; i++)
		{
			if (!itemPlaced)
			{
				if (playerInventory[i].type == itemToGive.type)
				{
					playerInventory[i].stack += stack;
					itemPlaced = true;
				}
			}
		}

		//Second, we check if there's an empty slot
		for (int i = 0; i < 5; i++)
		{
			if (!itemPlaced)
			{
				if (playerInventory[i].type == (int)Item.ItemTypes.Air)
				{
					playerInventory[i] = itemToGive;
					itemPlaced = true;
				}
			}
		}

		//Lastly, if the item can't be picked up, throw out whatever you're holding
		if (!itemPlaced)
		{
			if (selectedItem.type != (int)Item.ItemTypes.Air)
			{
				PackedScene itemScene = GD.Load<PackedScene>("res://Scenes/Environment/Items/" + selectedItem.name + ".tscn");
				Node2D droppedItem = itemScene.Instance() as Node2D;
				droppedItem.Set("initializeTimer", 30);
				droppedItem.Set("itemType", selectedItem.type);
				droppedItem.Set("amount", selectedItem.stack);
				mapYSort.AddChild(droppedItem);
				droppedItem.GlobalPosition = Player.player.GlobalPosition;
			}
			playerInventory[selectedInventorySlot] = itemToGive;
		}

		gameData.EmitSignal(nameof(UpdateInventorySlotDrawings));
	}

	public static void ConsumeItem(Item item, int amount = 1)
	{
		for (int i = 0; i < playerInventory.Length; i++)
		{
			if (playerInventory[i].type == item.type)
			{
				playerInventory[i].stack -= amount;
				if (playerInventory[i].stack <= 0)
				{
					playerInventory[i] = Item.itemList[(int)Item.ItemTypes.Air];
				}
			}
		}

		gameData.EmitSignal(nameof(UpdateInventorySlotDrawings));
	}

	public static void SpawnDeathClouds(Vector2 pos)
	{
		AnimatedSprite clouds = PackedScenes.packedScenesClass.deathClouds.Instance() as AnimatedSprite;
		mapYSort.AddChild(clouds);
		clouds.GlobalPosition = pos;
		clouds.Play("default");
	}
}
