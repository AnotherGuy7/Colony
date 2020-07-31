using Godot;
using System;

public class SceneSwitcher : Node
{
	public Node CurrentScene { get; set; }
	public static SceneSwitcher sceneSwitcher;

	private AnimationPlayer transitionPlayer;

	private PackedScene whatToLoad;
	private int spawnPointNumber;
	private string spawnDirection;

	public override void _Ready()
	{
		sceneSwitcher = this;
		Viewport root = GetTree().Root;
		CurrentScene = root.GetChild(root.GetChildCount() - 1);
		transitionPlayer = GetNode<AnimationPlayer>("Layer2/TransitionPlayer");
	}

	public void GotoScene(PackedScene sceneToLoad, int pointNum, string direction)
	{
		// This function will usually be called from a signal callback,
		// or some other function from the current scene.
		// Deleting the current scene at this point is
		// a bad idea, because it may still be executing code.
		// This will result in a crash or unexpected behavior.

		// The solution is to defer the load to a later time, when
		// we can be sure that no code from the current scene is running:

		transitionPlayer.Play("OutTransition");
		whatToLoad = sceneToLoad;
		spawnPointNumber = pointNum;
		spawnDirection = direction;
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
		}

		GameData.gameData.EmitSignal(nameof(GameData.SwitchedMaps), spawnPointNumber, spawnDirection);

		transitionPlayer.Play("InTransition");

	}

	private void OnTransitionDone(String anim_name)
	{
		if (anim_name == "InTransition")
		{
		}
		if (anim_name == "OutTransition")
		{
			CallDeferred(nameof(DeferredGotoScene));
		}
	}
}
