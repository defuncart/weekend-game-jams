/*
 *	Written by James Leahy. (c) 2017 DeFunc Art.
 *	https://github.com/defuncart/
 */
using DeFuncArt.Serialization;
using UnityEngine;

/// <summary>DataManager is responsible for maintaining the project's data files.</summary>
public static class DataManager
{
	/// <summary>Initializes the project's data files.</summary>
	public static void Initialize()
	{
		PlayerPreferencesKeys.Initialize();
	}

	/// <summary>Deletes the project's data files.</summary>
	public static void Delete()
	{
		PlayerPreferences.DeleteAll();
	}

	/// <summary>Verifies the project's data files, re-creating them if necessary.</summary>
	public static void Verify()
	{
	}

	/// <summary>Reloads the project's data files.</summary>
	public static void ReloadData()
	{
		Delete();
		Initialize();
	}
}
