using Godot;
using System;
using System.Collections.Generic;

public class ShopMenu : Control
{
	private static ShopMenu shopMenu;
	private Label Text;
	private Label nameLabel;
	private Panel shopDialog;
	private Panel shopPanel;
	private VBoxContainer itemButtonsContainer;
	private AudioStreamPlayer talkSound;

	public static List<int> shopArray = new List<int>();
	public static List<int> sellArray = new List<int>();
	public static List<Button> shopButtons = new List<Button>();
	public static List<Label> shopPrices = new List<Label>();

	/*private string currentText = "";
	private string currentName = "";*/
	private float textPercentage = 0f;
	private bool playerSelling = false;

	public override void _Ready()
	{
		shopMenu = this;
		Text = GetNode<Label>("Layer2/ShopDialog/Text");
		nameLabel = GetNode<Label>("Layer2/ShopDialog/NameLabel");
		shopDialog = GetNode<Panel>("Layer2/ShopDialog");
		shopPanel = GetNode<Panel>("Layer2/ShopPanel");
		talkSound = GetNode<AudioStreamPlayer>("TalkSound");

		itemButtonsContainer = GetNode<VBoxContainer>("Layer2/ShopPanel/ItemButtonsContainer");

		foreach (Button button in itemButtonsContainer.GetChildren())
		{
			shopButtons.Add(button);
		}
		foreach (object child in shopPanel.GetChildren())
		{
			if (child is Label)
			{
				Label price = child as Label;
				shopPrices.Add(price);
			}
		}
	}

	public override void _Process(float delta)
	{
		if (GameData.isPlayerTalking)
		{
			if (textPercentage < 1f)
			{
				textPercentage += 0.005f;
			}
			Text.PercentVisible = textPercentage;
		}
		if (shopPanel.Visible && Input.IsKeyPressed((int)KeyList.Escape))
		{
			shopPanel.Visible = false;
		}
	}

	public static void StartShop(string text, string speakerName, int[] whatToSell)
	{
		foreach (int itemType in whatToSell)
		{
			shopArray.Add(itemType);
		}
		foreach (Item item in GameData.playerInventory)
		{
			sellArray.Add(item.type);
		}
		shopMenu.nameLabel.Text = speakerName;
		shopMenu.Say(text);
		shopMenu.shopDialog.Visible = true;
	}

	private void ApplyShop()
	{
		for (int i = 0; i < 5; i++)
		{
			Item item = Item.itemsDict[shopArray[i]];
			if (item.type != Item.Air)
			{
				shopButtons[i].Icon = item.sprite;
				shopButtons[i].Text = item.name;
				shopPrices[i].Text = item.buyPrice.ToString();
				shopButtons[i].Visible = true;
				shopPrices[i].Visible = true;
			}
			if (item.type == Item.Air)
			{
				shopButtons[i].Visible = false;
				shopPrices[i].Visible = false;
			}
		}
		playerSelling = false;
		shopPanel.Visible = true;
	}

	private void ApplySellShop()
	{
		for (int i = 0; i < GameData.MaxInventorySlots; i++)
		{
			Item item = GameData.playerInventory[i];
			if (item.type != Item.Air)
			{
				shopButtons[i].Icon = item.sprite;
				shopButtons[i].Text = item.name;
				shopPrices[i].Text = item.buyPrice.ToString();
				shopButtons[i].Visible = true;
				shopPrices[i].Visible = true;
			}
			if (item.type == Item.Air)
			{
				shopButtons[i].Visible = false;
				shopPrices[i].Visible = false;
			}
		}
		playerSelling = true;
		shopPanel.Visible = true;
	}

	//Signals
	private void OnAction1Pressed()
	{
		ApplyShop();
	}

	private void OnAction2Pressed()
	{
		ApplySellShop();
	}

	private void OnAction3Pressed()
	{
		shopArray.Clear();
		sellArray.Clear();
		shopDialog.Visible = false;
		shopPanel.Visible = false;
		GameData.isPlayerTalking = false;
	}

	private void OnItemButton1Pressed()
	{
		int index = 0;
		Item item = Item.itemsDict[shopArray[index]];
		if (playerSelling)
		{
			GameData.playerCurrency += item.buyPrice * item.stack;
			GameData.playerInventory[index].stack = 0;
			GameData.playerInventory[index].type = Item.Air;
		}
		else
		{
			bool itemPlaced = Item.TryAddItemToInventory(item.type, 1);
			if (!itemPlaced)
			{
				Say("Sorry, but it seems that your inventory is full... Why not sell me some of those items you have there?");
			}
			else
			{
				Say("Pleasure doing business with you!");
			}
		}
	}


	private void OnItemButton2Pressed()
	{
		int index = 1;
		Item item = Item.itemsDict[shopArray[index]];
		if (playerSelling)
		{
			GameData.playerCurrency += item.buyPrice * item.stack;
			GameData.playerInventory[index].stack = 0;
			GameData.playerInventory[index].type = (int)Item.Air;
		}
		else
		{
			bool itemPlaced = Item.TryAddItemToInventory(item.type, 1);
			if (!itemPlaced)
			{
				Say("Sorry, but it seems that your inventory is full... Why not sell me some of those items you have there?");
			}
			else
			{
				Say("Pleasure doing business with you!");
			}
		}
	}


	private void OnItemButton3Pressed()
	{
		int index = 2;
		Item item = Item.itemsDict[shopArray[index]];
		if (playerSelling)
		{
			GameData.playerCurrency += item.buyPrice * item.stack;
			GameData.playerInventory[index].stack = 0;
			GameData.playerInventory[index].type = (int)Item.Air;
		}
		else
		{
			bool itemPlaced = Item.TryAddItemToInventory(item.type, 1);
			if (!itemPlaced)
			{
				Say("Sorry, but it seems that your inventory is full... Why not sell me some of those items you have there?");
			}
			else
			{
				Say("Pleasure doing business with you!");
			}
		}
	}


	private void OnItemButton4Pressed()
	{
		int index = 3;
		Item item = Item.itemsDict[shopArray[index]];
		if (playerSelling)
		{
			GameData.playerCurrency += item.buyPrice * item.stack;
			GameData.playerInventory[index].stack = 0;
			GameData.playerInventory[index].type = (int)Item.Air;
		}
		else
		{
			bool itemPlaced = Item.TryAddItemToInventory(item.type, 1);
			if (!itemPlaced)
			{
				Say("Sorry, but it seems that your inventory is full... Why not sell me some of those items you have there?");
			}
			else
			{
				Say("Pleasure doing business with you!");
			}
		}
	}


	private void OnItemButton5Pressed()
	{
		int index = 4;
		Item item = Item.itemsDict[shopArray[index]];
		if (playerSelling)
		{
			GameData.playerCurrency += item.buyPrice * item.stack;
			GameData.playerInventory[index].stack = 0;
			GameData.playerInventory[index].type = (int)Item.Air;
		}
		else
		{
			bool itemPlaced = Item.TryAddItemToInventory(item.type, 1);
			if (!itemPlaced)
			{
				Say("Sorry, but it seems that your inventory is full... Why not sell me some of those items you have there?");
			}
			else
			{
				Say("Pleasure doing business with you!");
			}
		}
	}

	private void Say(string whatToSay)
	{
		textPercentage = 0f;
		Text.Text = whatToSay;
	}
}
