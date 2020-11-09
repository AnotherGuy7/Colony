using Godot;
using System.Collections.Generic;

public class Item : Node2D
{
    public string name = "";
    public int type = 0;
    public Vector2 offsetInInventory = Vector2.Zero;
    public int stack = 1;
    public int consumeAmount = 0;       //for things that need consuming
    public int buyPrice = 10;
    public int useType = 1;
    public Texture sprite;
    public AudioStream useSound;

    //Things for weapons
    public int weaponDamage;

    //Things for healing items
    public int healAmount = 0;

    public const int NoUse = 0;
    public const int Weapon = 1;
    public const int Healing = 2;
    public const int Equip = 3;

    //public static List<Item> itemList = new List<Item>();

    //public static readonly Dictionary<int, Item> itemDict = new Dictionary<int, Item>();
    //public static readonly Dictionary itemDict = new Dictionary();

    public static List<Item> itemList = new List<Item>();

    internal enum ItemTypes
    { 
        Air,
        Apple,
        Sword,
        Bow,
        Rapier,
        SmallHealthVial
    }

    public override void _Ready()
    {
        Item air = new Item();
        air.name = "Nothing";
        air.type = (int)ItemTypes.Air;
        air.useType = NoUse;
        air.sprite = GetTexture(air.name);
        itemList.Add(air);

        Item apple = new Item();
        apple.name = "Apple";
        apple.type = (int)ItemTypes.Apple;
        apple.useType = Healing;
        apple.sprite = GetTexture(apple.name);
        apple.consumeAmount = 1;
        apple.healAmount = 1;
        apple.buyPrice = 5;
        apple.useSound = GetSound("Eat");
        itemList.Add(apple);

        Item sword = new Item();
        sword.name = "Sword";
        sword.type = (int)ItemTypes.Sword;
        sword.useType = Weapon;
        sword.sprite = GetTexture("Sword1");
        sword.buyPrice = 20;
        sword.useSound = GetSound("SwordSwing");
        itemList.Add(sword);

        Item bow = new Item();
        bow.name = "Bow";
        bow.type = (int)ItemTypes.Bow;
        bow.useType = Weapon;
        bow.sprite = GetTexture(bow.name);
        bow.buyPrice = 15;
        itemList.Add(bow);

        Item rapier = new Item();
        rapier.name = "Rapier";
        rapier.type = (int)ItemTypes.Rapier;
        rapier.useType = Weapon;
        rapier.sprite = GetTexture("Rapier");
        rapier.buyPrice = 35;
        itemList.Add(rapier);

        Item smallHealthVial = new Item();
        smallHealthVial.name = "Samll Health Vial";
        smallHealthVial.type = (int)ItemTypes.SmallHealthVial;
        smallHealthVial.useType = Healing;
        smallHealthVial.sprite = GetTexture("SmallHealthVial");
        smallHealthVial.consumeAmount = 1;
        smallHealthVial.healAmount = 2;
        smallHealthVial.buyPrice = 20;
        itemList.Add(smallHealthVial);
    }

    private Texture GetTexture(string textureName)
    {
        Texture texture = (Texture)GD.Load("res://Sprites/UI/Items/" + textureName + ".png");
        if (texture == null)
        {
            texture = (Texture)GD.Load("res://Sprites/UI/Items/Nothing.png");
        }
        return texture;
    }

    private AudioStream GetSound(string soundName)
    {
        AudioStream sound = (AudioStream)GD.Load("res://Sounds/ItemSFX/" + soundName + ".wav");
        /*if (sound == null)
        {

        }*/
        return sound;
    }

    //Static methods
    public static void AddItemToInventory(Item itemToGive, int stack)
    {
        Item selectedItem = GameData.playerInventory[GameData.selectedInventorySlot];
        bool itemPlaced = false;

        //First, we check if a stack of the item exists
        for (int i = 0; i < 5; i++)
        {
            if (!itemPlaced)
            {
                if (GameData.playerInventory[i].type == itemToGive.type)
                {
                    GameData.playerInventory[i].stack += stack;
                    itemPlaced = true;
                }
            }
        }

        //Second, we check if there's an empty slot
        for (int i = 0; i < 5; i++)
        {
            if (!itemPlaced)
            {
                if (GameData.playerInventory[i].type == (int)ItemTypes.Air)
                {
                    GameData.playerInventory[i] = itemToGive;
                    itemPlaced = true;
                }
            }
        }

        //Lastly, if the item can't be picked up, throw out whatever you're holding
        if (!itemPlaced)
        {
            if (selectedItem.type != (int)ItemTypes.Air)
            {
                PackedScene itemScene = GD.Load<PackedScene>("res://Scenes/Environment/Items/" + selectedItem.name + ".tscn");
                Node2D droppedItem = itemScene.Instance() as Node2D;
                droppedItem.Set("initializeTimer", 30);
                droppedItem.Set("itemType", selectedItem.type);
                droppedItem.Set("amount", selectedItem.stack);
                GameData.mapYSort.AddChild(droppedItem);
                droppedItem.GlobalPosition = Player.player.GlobalPosition;
            }
            GameData.playerInventory[GameData.selectedInventorySlot] = itemToGive;
        }

        GameData.gameData.EmitSignal(nameof(GameData.UpdateInventorySlotDrawings));
    }

    public static void AddItemToInventory(int itemType, int stack)
    {
        Item selectedItem = GameData.playerInventory[GameData.selectedInventorySlot];
        Item itemToRecieve = itemList[itemType];
        bool itemPlaced = false;

        //First, we check if a stack of the item exists
        for (int i = 0; i < 5; i++)
        {
            if (!itemPlaced)
            {
                if (GameData.playerInventory[i].type == itemType)
                {
                    GameData.playerInventory[i].stack += stack;
                    itemPlaced = true;
                }
            }
        }

        //Second, we check if there's an empty slot
        for (int i = 0; i < 5; i++)
        {
            if (!itemPlaced)
            {
                if (GameData.playerInventory[i].type == (int)ItemTypes.Air)
                {
                    GameData.playerInventory[i] = itemToRecieve;
                    itemPlaced = true;
                }
            }
        }

        //Lastly, if the item can't be picked up, throw out whatever you're holding
        if (!itemPlaced)
        {
            if (selectedItem.type != (int)ItemTypes.Air)
            {
                PackedScene itemScene = GD.Load<PackedScene>("res://Scenes/Environment/Items/" + selectedItem.name + ".tscn");
                Node2D droppedItem = itemScene.Instance() as Node2D;
                droppedItem.Set("initializeTimer", 30);
                droppedItem.Set("itemType", selectedItem.type);
                droppedItem.Set("amount", selectedItem.stack);
                GameData.mapYSort.AddChild(droppedItem);
                droppedItem.GlobalPosition = Player.player.GlobalPosition;
            }
            GameData.playerInventory[GameData.selectedInventorySlot] = itemToRecieve;
        }

        GameData.gameData.EmitSignal(nameof(GameData.UpdateInventorySlotDrawings));
    }

    public static void ConsumeItem(Item item, int amount = 1)
    {
        for (int i = 0; i < GameData.playerInventory.Length; i++)
        {
            if (GameData.playerInventory[i].type == item.type)
            {
                GameData.playerInventory[i].stack -= amount;
                if (GameData.playerInventory[i].stack <= 0)
                {
                    GameData.playerInventory[i] = itemList[(int)ItemTypes.Air];
                }
            }
        }

        GameData.gameData.EmitSignal(nameof(GameData.UpdateInventorySlotDrawings));
    }
}
