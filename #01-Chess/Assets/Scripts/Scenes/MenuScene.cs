/*
 *	Written by James Leahy. (c) 2017 DeFunc Art.
 *	https://github.com/defuncart/
 */
using DeFuncArt.Serialization;
using DeFuncArt.UI;
using UnityEngine;
using UnityEngine.Assertions;
using UnityEngine.SceneManagement;

/// <summary>This script controls the menu scene.</summary>
public class MenuScene : MonoBehaviour
{
	#region Properties

	/// <summary>The player1 radio button group.</summary>
	[SerializeField] private DARadioButtonGroup player1RadioButtonGroup;
	/// <summary>The player1 radio button group.</summary>
	[SerializeField] private DARadioButtonGroup player2RadioButtonGroup;
	/// <summary>The play button.</summary>
	[SerializeField] private DAButton playButton;

	#endregion

	#region Initialization

	/// <summary>EDITOR ONLY: Callback when the script is loaded or a value is changed in the inspector.</summary>
	private void OnValidate()
	{
		Assert.IsNotNull(player1RadioButtonGroup);
		Assert.IsNotNull(player2RadioButtonGroup);
		Assert.IsNotNull(playButton);
	}

	/// <summary>Callback when the instance starts.</summary>
	private void Start()
	{
		//initialize the radio button groups
		player1RadioButtonGroup.ResetForIndex(PlayerPreferences.GetInt(PlayerPreferencesKeys.player1));
		player2RadioButtonGroup.ResetForIndex(PlayerPreferences.GetInt(PlayerPreferencesKeys.player2));
	}

	#endregion

	#region Callbacks

	/// <summary>Callback when the PlayButton is pressed.</summary>
	public void PlayButtonPressed()
	{
		playButton.interactable = false;
		//update player1 and player2 preferences and load the game scene
		PlayerPreferences.SetInt(PlayerPreferencesKeys.player1, player1RadioButtonGroup.selectedIndex);
		PlayerPreferences.SetInt(PlayerPreferencesKeys.player2, player2RadioButtonGroup.selectedIndex);
		SceneManager.LoadScene(SceneBuildIndeces.GameScene);
	}

	#endregion
}
