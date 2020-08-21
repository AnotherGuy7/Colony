using Godot;
using System;
using System.ComponentModel;
using System.Linq;

public class GameData : Node2D
{
	[Signal]
	public delegate void SwitchedMaps(int positionToSpawnAt, string direction = "Front");		//this is sent after a map change

	[Signal]
	public delegate void UpdateInventorySlotDrawings();       //this is sent after the player receives an item, or uses one

	public static GameData gameData;
	public static YSort mapYSort;

	//Player stats
	public static int playerMaxHealth = 4;
	public static int playerHealth = 8;
	public static int playerDamage = 1;
	public static int playerCurrency = 100;
	//public static Item[] playerInventory = new Item[5] { Item.itemDict[(int)Item.ItemTypes.Air], Item.itemDict[(int)Item.ItemTypes.Air], Item.itemDict[(int)Item.ItemTypes.Air], Item.itemDict[(int)Item.ItemTypes.Air], Item.itemDict[(int)Item.ItemTypes.Air]};
	public static Item[] playerInventory = new Item[5];

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
		for (int i = 0; i < playerInventory.Length; i++)
		{
			if (i == 0)
				playerInventory[i] = Item.itemList[(int)Item.ItemTypes.Sword];
			else
				playerInventory[i] = Item.itemList[(int)Item.ItemTypes.Air];
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
		if (!itemPlaced)
		{
			if (selectedItem.type == itemToGive.type)
			{
				playerInventory[selectedInventorySlot].stack += stack;
			}
			else
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
}
