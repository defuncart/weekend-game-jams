/*
 *	Written by James Leahy. (c) 2017 DeFunc Art.
 *	https://github.com/defuncart/
 */
using UnityEngine;

/// <summary>A struct of game constants.</summary>
public struct Constants
{
	/// <summary>The number of pieces types.</summary>
	public const int NUMBER_TYPE_PIECES = 6;
}

/// <summary>A struct of animation duration constants.</summary>
public struct AnimationDuration
{
	/// <summary>The duration of the loading scene.</summary>
	public const float LOADING_SCENE = 3.5f;
}

/// <summary>A struct of game store variables.</summary>
public struct GameStore {}

/// <summary>A struct of scene build indeces.</summary>
public struct SceneBuildIndeces
{
	/// <summary>The loading scene.</summary>
	public const int LoadingScene = 0;
	/// <summary>The menu scene.</summary>
	public const int MenuScene = 1;
	/// <summary>The game scene.</summary>
	public const int GameScene = 2;
}

/// <summary>A struct of Tags.</summary>
public struct Tags {}

/// <summary>A struct of colors used in the game.</summary>
public struct GameColors
{
	/// <summary>A light gray color used as the lighter color for squares on the chess board.</summary>
	public static Color cellLight
	{
		get { return new Color(0.8f, 0.8f, 0.8f); }
	}

	/// <summary>A dark gray color used as the darker color for squares on the chess board.</summary>
	public static Color cellDark
	{
		get { return new Color(0.6f, 0.6f, 0.6f); }
	}

	/// <summary>A green color used to highlight possible moves.</summary>
	public static Color cellHighlight
	{
		get { return new Color(0.2f, 0.8f, 0.2f); }
	}

	/// <summary>A red color used to when there are no possible moves.</summary>
	public static Color cellNoMoves
	{
		get { return new Color(0.8f, 0.2f, 0.2f); }
	}
}
