using Godot;

public class House1 : StaticBody2D
{
	[Export]
	public string mapToGoTo;

	[Export]
	public Texture doorClosed;

	[Export]
	public Texture doorOpen;

	private Sprite doorSprite;

	public override void _Ready()
	{
		doorSprite = GetNode<Sprite>("Door/DoorSprite");
	}

	private void OnDoorBodyEntered(object body)
	{
		if (body == Player.player)
		{
			SceneSwitcher.sceneSwitcher.GotoScene(mapToGoTo, 1, "Back");
		}
	}

	private void OnOpenAreaBodyEntered(object body)
	{
		if (body == Player.player)
		{
			doorSprite.Texture = doorOpen;
		}
	}


	private void OnOpenAreaBodyExited(object body)
	{
		if (body == Player.player)
		{
			doorSprite.Texture = doorClosed;
		}
	}
}
