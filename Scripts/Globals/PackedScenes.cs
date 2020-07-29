using Godot;
using System;

public class PackedScenes : Node2D
{
	public static PackedScenes packedScenesClass;

	[Export]
	public PackedScene Woodville;

	[Export]
	public PackedScene WoodvilleHouse1;

	[Export]
	public PackedScene WoodvilleHouse2;

	public override void _Ready()
	{
		packedScenesClass = this;
	}
}
