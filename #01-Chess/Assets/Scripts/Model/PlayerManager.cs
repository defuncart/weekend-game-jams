/*
 *	Written by James Leahy. (c) 2017 DeFunc Art.
 *	https://github.com/defuncart/
 */
using DeFuncArt.ExtensionMethods;
using DeFuncArt.Utilities;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>A serializable singleton manager which is responsible for saving the player's data to disk.</summary>
[System.Serializable]
public class PlayerManager : SerializableSingleton<PlayerManager>
{
	#region Properties

	#endregion

	#region Initialization

	/// <summary>Initializes an instance of the class.</summary>
	protected PlayerManager()
	{
		Save();
	}

	#endregion

	#region Methods

	#endregion
}
