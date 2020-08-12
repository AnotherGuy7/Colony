using Godot;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;

public class UI : Control
{
	[Export]
	public Texture halfHeart;

	[Export]
	public Texture emptyHeart;

	[Export]
	public Texture fullHeart;

	[Export]
	public Texture inactiveSlot;

	[Export]
	public Texture activeSlot;

	private Panel mapPanel;
	private TextureRect playerMarker;
	private Panel heartsPanel;
	private Panel inventoryPanel;

	private int currentHeart = 0;

	private List<TextureRect> inventorySlots = new List<TextureRect>();
	private List<TextureRect> hearts = new List<TextureRect>();

	public override void _Ready()
	{
		mapPanel = GetNode<Panel>("Layer1/MapPanel");
		playerMarker = GetNode<TextureRect>("Layer1/MapPanel/PlayerMarkerCenter/PlayerMarker");
		heartsPanel = GetNode<Panel>("Layer1/HealthBar");
		inventoryPanel = GetNode<Panel>("Layer1/InventoryPanel");
		GameData.gameData.Connect(nameof(GameData.UpdateInventorySlotDrawings), this, nameof(UpdateInventoryDrawings));

		foreach (TextureRect heart in heartsPanel.GetChildren())
		{
			hearts.Add(heart);
		}
		foreach (TextureRect slot in inventoryPanel.GetChildren())
		{
			inventorySlots.Add(slot);
		}
	}

	private void UpdateInventoryDrawings()
	{
		for (int i = 0; i < GameData.playerInventory.Length; i++)
		{
			Item item = GameData.playerInventory[i];
			TextureRect currentSlot = inventorySlots[i];
			TextureRect currentSlotItemRect = (TextureRect)inventorySlots[i].GetNode(inventorySlots[i].GetPath() + "/ItemTexture");
			Label currentSlotStack = (Label)inventorySlots[i].GetNode(inventorySlots[i].GetPath() + "/ItemStack");
			currentSlotItemRect.Texture = item.sprite;

			if (item.stack > 1)
				currentSlotStack.Text = item.stack.ToString();
			else
				currentSlotStack.Text = "";

			if (i == GameData.selectedInventorySlot)
				currentSlot.Texture = activeSlot;
			else
				currentSlot.Texture = inactiveSlot;
		}
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("map"))
		{
			mapPanel.Visible = !mapPanel.Visible;
			Notification(NotificationDraw);
		}
		currentHeart = GameData.playerHealth / 2;
		for (int i = 0; i < hearts.Count; i++)		//change this so that it changes upon a signal
		{
			if (i < GameData.playerMaxHealth)
			{
				hearts[i].Visible = true;
			}
			else
			{
				hearts[i].Visible = false;
			}
			if (i > currentHeart)
				hearts[i].Texture = emptyHeart;
			if (i < currentHeart)
				hearts[i].Texture = fullHeart;
			if (i == currentHeart)
			{
				if (GameData.playerHealth % 2f == 1)
				{
					hearts[i].Texture = fullHeart;
				}
				else
				{
					hearts[i].Texture = halfHeart;
				}
			}
		}
	}
}
