using Godot;
using System;

public class SceneSwitcher : Node
{
	public Node CurrentScene { get; set; }
	public static SceneSwitcher sceneSwitcher;

	private AnimationPlayer transitionPlayer;
	private AnimationPlayer flagPlayer;
	private RichTextLabel locationNameLabel;

	private PackedScene whatToLoad;
	private int spawnPointNumber;
	private string spawnDirection;
	private string locationName;
	private bool showMapFlag;

	public override void _Ready()
	{
		sceneSwitcher = this;
		Viewport root = GetTree().Root;
		CurrentScene = root.GetChild(root.GetChildCount() - 1);
		flagPlayer = GetNode<AnimationPlayer>("Layer2/FlagPlayer");
		transitionPlayer = GetNode<AnimationPlayer>("Layer2/TransitionPlayer");
		locationNameLabel = GetNode<RichTextLabel>("Layer2/FlagPlayer/FlagSprite/LocationNameLabel");
	}

	public void GotoScene(string sceneToLoad, int pointNum, string direction, bool showEntranceFlag = false, string modLocationName = "")
	{
		// This function will usually be called from a signal callback,
		// or some other function from the current scene.
		// Deleting the current scene at this point is
		// a bad idea, because it may still be executing code.
		// This will result in a crash or unexpected behavior.

		// The solution is to defer the load to a later time, when
		// we can be sure that no code from the current scene is running:

		transitionPlayer.Play("OutTransition");
		whatToLoad = PackedScenes.scenesDict[sceneToLoad];
		spawnPointNumber = pointNum;
		spawnDirection = direction;
		locationName = GameData.playerLocation = sceneToLoad;
		showMapFlag = showEntranceFlag;
		if (modLocationName != "")
		{
			locationName = modLocationName;
		}
		GameData.transitioning = true;
	}

	public void DeferredGotoScene()
	{
		// It is now safe to remove the current scene
		CurrentScene.Free();

		// Instance the new scene.
		CurrentScene = whatToLoad.Instance();

		// Add it to the active scene, as child of root.
		GetTree().Root.AddChild(CurrentScene);

		// Optionally, to make it compatible with the SceneTree.change_scene() API.
		GetTree().CurrentScene = CurrentScene;

		foreach (object allChildren in GetTree().CurrentScene.GetChildren())
		{
			if (allChildren.GetType().ToString() == "Godot.YSort")
			{
				YSort ySort = allChildren as YSort;
				GameData.mapYSort = ySort;
			}
			if (allChildren.GetType().ToString() == "Godot.Position2D")		//setting camrea limits upon entering scene
			{
				Position2D positions = allChildren as Position2D;
				if (positions.Name.Contains("CameraLimit"))		//only for maps that need it, if 
				{
					Position2D cameraLimits = positions;
					switch (cameraLimits.Name)
					{
						case "CameraLimitTop":
							Player.playerCam.LimitTop = (int)cameraLimits.GlobalPosition.y;
							break;
						case "CameraLimitLeft":
							Player.playerCam.LimitLeft = (int)cameraLimits.GlobalPosition.x;
							break;
						case "CameraLimitBottom":
							Player.playerCam.LimitBottom = (int)cameraLimits.GlobalPosition.y;
							break;
						case "CameraLimitRight":
							Player.playerCam.LimitRight = (int)cameraLimits.GlobalPosition.x;
							break;
					}
				}
			}
		}

		GameData.transitioning = false;
		Player.player.Visible = true;       //For cases where the player is invisible (The rest is resetting variables)
		Player.moveDuringTransition = false;
		Player.transitionVelocityVector = Vector2.Zero;
		GameData.gameData.EmitSignal(nameof(GameData.SwitchedMaps), spawnPointNumber, spawnDirection);

		transitionPlayer.Play("InTransition");


	}

	private void OnTransitionDone(String anim_name)
	{
		if (anim_name == "InTransition")
		{
			if (showMapFlag)
			{
				locationNameLabel.BbcodeText = "[wave]" + locationName;
				flagPlayer.Play("FlagAnim");
				showMapFlag = false;
			}
		}
		if (anim_name == "OutTransition")
		{
			CallDeferred(nameof(DeferredGotoScene));
		}
	}
}
