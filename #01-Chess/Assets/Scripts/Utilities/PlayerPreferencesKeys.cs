/*
 * 	Written by James Leahy. (c) 2017 DeFunc Art.
 * 	https://github.com/defuncart/
 */
using DeFuncArt.Serialization;

/// <summary>Here the project's PlayerPreferences' keys are declared and initialized.</summary>
public static class PlayerPreferencesKeys//: PlayerPreferences.Keys
{
	/// <summary>Whether the game was previously launched.</summary>
	public const string previouslyLaunched = "previouslyLaunched";
	/// <summary>The Player1 player type.</summary>
	public const string player1 = "player1";
	/// <summary>The Player2 player type.</summary>
	public const string player2 = "player2";

	/// <summary>Initializes required key-value pairs to their initial values.</summary>
	public static void Initialize()
	{
		PlayerPreferences.SetBool(previouslyLaunched, true);
		PlayerPreferences.SetInt(player1, 0);
		PlayerPreferences.SetInt(player2, 0);
	}
}
