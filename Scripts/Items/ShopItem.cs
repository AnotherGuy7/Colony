using Godot;
using System;

public class ShopItem : Sprite
{
	[Export]
	public string itemName;

	private Label priceTag;
	private Item itemToSell;
	private bool canBuy = false;

	public override void _Ready()
	{
		itemToSell = Item.itemsDict[Item.GetItemType(itemName)];
		Texture = itemToSell.sprite;
		priceTag = GetNode<Label>("PriceTag");
		priceTag.Text = "$" + itemToSell.buyPrice;
	}

	public override void _Input(InputEvent @event)
	{
		if (canBuy && Input.IsActionJustPressed("continue") && Item.SpaceAvailableInInventory() && GameData.playerCurrency >= itemToSell.buyPrice)
		{
			GameData.playerCurrency -= itemToSell.buyPrice;
			Item.AddItemToInventory(itemToSell, 1);
			Dispose();
		}
	}

	private void OnBuyAreaEntered(object body)
	{
		if (body == Player.player)
		{
			canBuy = true;
		}
	}


	private void OnBuyAreaExited(object body)
	{
		if (body == Player.player)
		{
			canBuy = false;
		}
	}
}
