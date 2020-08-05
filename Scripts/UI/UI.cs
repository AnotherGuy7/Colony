using Godot;
using System;

public class UI : Control
{
	public static TextureRect miniMapRect;

	private Panel mapPanel;
	private TextureRect map;
	private TextureRect playerMarker;
	private Position2D playerMarkerCenter;
	private Position2D cameraLimitLeft;
	private Position2D cameraLimitTop;
	private Position2D cameraLimitBottom;
	private Position2D cameraLimitRight;

	public override void _Ready()
	{
		miniMapRect = GetNode<TextureRect>("Layer1/MapPanel/Map");
		mapPanel = GetNode<Panel>("Layer1/MapPanel");
		map = GetNode<TextureRect>("Layer1/MapPanel/Map");
		playerMarker = GetNode<TextureRect>("Layer1/MapPanel/PlayerMarkerCenter/PlayerMarker");
	}

	public override void _Process(float delta)
	{
		if (Input.IsActionJustPressed("map"))
		{
			foreach (object allChildren in GetTree().CurrentScene.GetChildren())
			{
				if (allChildren.GetType().ToString() == "Godot.Position2D")
				{
					Position2D positions = allChildren as Position2D;
					if (positions.Name.Contains("CameraLimit"))
					{
						Position2D cameraLimits = positions;
						if (cameraLimits.Name == "CamreaLimitLeft")
						{
							cameraLimitLeft = cameraLimits;
						}
						if (cameraLimits.Name == "CamreaLimitTop")
						{
							cameraLimitTop = cameraLimits;
						}
					}
				}
			}
			Vector2 totalArea = new Vector2(cameraLimitRight.GlobalPosition.x - cameraLimitLeft.GlobalPosition.x, cameraLimitTop.GlobalPosition.y - cameraLimitBottom.GlobalPosition.y);
			float differenceX = map.RectSize.x / totalArea.x;
			float differenceY = map.RectSize.y / totalArea.y;
			Vector2 markerAddition = Vector2.Zero;
			markerAddition.x = (cameraLimitLeft.GlobalPosition.x + Player.player.GlobalPosition.x) * differenceX;
			markerAddition.y = (cameraLimitTop.GlobalPosition.y - Player.player.GlobalPosition.y) * differenceY;
			playerMarker.RectGlobalPosition = playerMarkerCenter.GlobalPosition + markerAddition;
			mapPanel.Visible = !mapPanel.Visible;
		}
	}
}
