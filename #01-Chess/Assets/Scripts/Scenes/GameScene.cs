/*
 *	Written by James Leahy. (c) 2017 DeFunc Art.
 *	https://github.com/defuncart/
 */
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>This script controls the game scene.</summary>
public class GameScene : MonoBehaviour
{
	#region Callbacks

	/// <summary>Callback when the exit button is pressed.</summary>
	public void ExitButtonPressed()
	{
		SceneManager.LoadScene(SceneBuildIndeces.MenuScene);
	}

	#endregion
}
