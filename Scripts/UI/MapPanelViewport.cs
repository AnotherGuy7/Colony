using Godot;

public class MapPanelViewport : Viewport
{
	private Camera2D mapCam;
	private Position2D centerPoint;
	private Panel mapPanel;
	private ViewportContainer container;

	public override void _Ready()
	{
		mapCam = GetNode<Camera2D>("MapCam");
		mapPanel = GetParent().GetParent() as Panel;
		centerPoint = mapPanel.GetNode<Position2D>("CenterPoint");

		container = GetParent() as ViewportContainer;
	}

    public override void _Input(InputEvent @event)
	{
		if (@event is InputEventMouseButton scroll)
		{
			if (scroll.IsPressed())
			{
				if (scroll.ButtonIndex == (int)ButtonList.WheelDown)
				{
					mapCam.Zoom = new Vector2(mapCam.Zoom.x + 0.05f, mapCam.Zoom.y + 0.05f);
				}
				if (scroll.ButtonIndex == (int)ButtonList.WheelUp)
				{
					mapCam.Zoom = new Vector2(mapCam.Zoom.x - 0.05f, mapCam.Zoom.y - 0.05f);
				}

				if (mapCam.Zoom.x > 0.9f)
				{
					mapCam.Zoom = new Vector2(0.9f, 0.9f);
				}
				if (mapCam.Zoom.x < 0.1f)
				{
					mapCam.Zoom = new Vector2(0.1f, 0.1f);
				}
			}
		}
		if (Godot.Input.IsMouseButtonPressed((int)ButtonList.Left))
        {
			MoveMapCam();
		}
		if (Godot.Input.IsActionJustPressed("map") && !UI.ui.uiAnimPlayer.IsPlaying() && UI.mapPanelActive)
		{
			UI.ui.uiAnimPlayer.Play("MapTransitionOut");
			SetProcessInput(false);
			HandleInputLocally = false;
			GuiDisableInput = true;
			UI.mapPanelActive = false;
		}
	}

	private void MoveMapCam()
    {
		Vector2 difference = mapPanel.GetGlobalMousePosition() - centerPoint.GlobalPosition;
		mapCam.Position -= difference.Normalized() * 2f * -mapCam.Zoom.x;
    }
}
