using Godot;
using System;

public class TitleScreen : Control
{
	private Panel settingsPanel;
	private Label resolutionNumber;

	private Vector2[] resolutionsArray = new Vector2[7] { new Vector2(256f, 150f), Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero, Vector2.Zero };
	private int resolutionsArrayIndex = 0;

	public override void _Ready()
	{
		settingsPanel = GetNode<Panel>("SettingsPanel");
		resolutionNumber = GetNode<Label>("SettingsPanel/ResolutionNumberLabel");
	}

	public override void _Input(InputEvent @event)
	{
		if (@event is InputEventKey)
		{
			InputEventKey keyPressed = @event as InputEventKey;
			if (keyPressed.Scancode == (int)KeyList.Escape)
			{
				if (settingsPanel.Visible)
				{
					settingsPanel.Visible = false;
				}
			}
		}
	}

	private void OnPlayButtonPressed()
	{
		SceneSwitcher.sceneSwitcher.GotoScene(PackedScenes.packedScenesClass.Woodville, 1, "Front");
	}
	
	
	private void OnSettingsButtonPressed()
	{
		settingsPanel.Visible = true;
	}

	private void OnLowerResPresse()
	{
		resolutionsArrayIndex -= 1;
		if (resolutionsArrayIndex < 0)
		{
			resolutionsArrayIndex = resolutionsArray.Length - 1;
		}
		UpdateResText();
	}

	private void OnHigherResPressed()
	{
		resolutionsArrayIndex += 1;
		if (resolutionsArrayIndex < resolutionsArray.Length)
		{
			resolutionsArrayIndex = 0;
		}
		UpdateResText();
	}


	private void OnFullscreenPressed()
	{
		OS.WindowFullscreen = !OS.WindowFullscreen;
	}

	private void UpdateResText()
	{
		resolutionNumber.Text = resolutionsArray[resolutionsArrayIndex].x + " x " + resolutionsArray[resolutionsArrayIndex].y;
	}
}
