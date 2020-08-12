using Godot;
using Godot.Collections;
using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;

public class Item : Node2D
{
    public string name = "";
    public int type = 0;
    public Vector2 offsetInInventory = Vector2.Zero;
    public int stack = 1;
    public int consumeAmount = 0;       //for things that need consuming
    public int useType = 1;
    public Texture sprite;

    //Things for weapons

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
        Bow
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
        itemList.Add(apple);

        Item sword = new Item();
        sword.name = "Sword";
        sword.type = (int)ItemTypes.Sword;
        sword.useType = Weapon;
        sword.sprite = GetTexture("Sword1");
        itemList.Add(sword);

        Item bow = new Item();
        bow.name = "Bow";
        bow.type = (int)ItemTypes.Bow;
        bow.useType = Weapon;
        bow.sprite = GetTexture(bow.name);
        itemList.Add(bow);
    }

    private Texture GetTexture(string textureName)
    {
        Texture texture = (Texture)GD.Load("res://Sprites/UI/Items/" + textureName + ".png");
        return texture;
    }
}
