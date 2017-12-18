/*
 *	Written by James Leahy. (c) 2017 DeFunc Art.
 *	https://github.com/defuncart/
 */
using DeFuncArt.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

public class Player : MonoBehaviour
{
	#region Properties

	/// <summary>An enum denoting the type of players.</summary>
	public enum Type
	{
		/// <summary>A human player who physically makes moves using touch events.</summary>
		Human, 
		/// <summary>A computer player who makes moves automatically using an AI.</summary>
		Computer
	}
	/// <summary>The player type.</summary>
	public Type type;
	/// <summary>An enum denoting the possible player colors.</summary>
	public enum Color
	{
		/// <summary>Black means that all the player's pieces are black.</summary>
		Black, 
		/// <summary>White means that all the player's pieces are white.</summary>
		White
	}
	/// <summary>The player's color.</summary>
	public Player.Color color;
	/// <summary>Determines if the player is playing as white.</summary>
	public bool isWhite
	{
		get { return color == Player.Color.White; }
	}
	/// <summary>Determines if the player is playing as black.</summary>
	public bool isBlack
	{
		get { return color == Player.Color.Black; }
	}
	/// <summary>A list of board pieces that the player has captured.</summary>
	private List<BoardPiece> capturedPieces;
	/// <summary>Determine's the number of pieces that the player has captured.</summary>
	private int numberCapturedPieces
	{
		get { return capturedPieces.Count; }
	}
	/// <summary>The selected board piece.</summary>
	[HideInInspector] public BoardPiece selectedPiece;
	/// <summary>A selected board cell to move to.</summary>
	[HideInInspector] public BoardCell selectedCell;
	/// <summary>A list of the valid moves for the player's selectedPiece.</summary>
	[HideInInspector] public List<int[]> validMoves;

	#endregion

	#region Initialization

	/// <summary>Callback when the instance awakes.</summary>
	private void Awake()
	{
		capturedPieces = new List<BoardPiece>();
	}

	#endregion

	#region Methods

	/// <summary>Resets the player.</summary>
	public void Reset()
	{
		foreach(BoardPiece piece in capturedPieces)
		{
			Destroy(piece.gameObject);
		}
	}

	/// <summary>Sets the player's type from a given integer.</summary>
	/// <param name="type">The type.</param>
	public void SetType(int type)
	{
		Assert.IsTrue(type == 0 || type == 1);
		this.type = (Type)type;
	}

	/// <summary>Adds a piece that the player captured.</summary>
	/// <param name="piece">The captured piece.</param>
	public void AddCapturedPiece(BoardPiece piece)
	{
		//parent the piece to the player
		piece.transform.parent = this.transform;
		//determine (x, y) position and place just off the board
		int x = numberCapturedPieces % GameBoard.COLUMNS;
		x = (isWhite ? x : GameBoard.COLUMNS - 1 - x);
		float y = (numberCapturedPieces / GameBoard.COLUMNS) * (isWhite ? -1 : 1);
		piece.transform.localPosition = new Vector3(x, y, 0f);
		//if the player is white, they then captured a black piece => rotate facing downwards
		//otherwise player is black and they captured a white piece => rotate facing upwards
		piece.transform.eulerAngles = (isWhite ? Vector3.zero : new Vector3(0, 0, 180));
		//add piece to player's captured piece's list
		capturedPieces.Add(piece);
	}

	/// <summary>Determines whether an (x, y) pair is a valid moves.</summary>
	/// <param name="x">The x-value.</param>
	/// <param name="y">The y-value.</param>
	public bool IsValidMove(int x, int y)
	{
		for(int i=0; i < validMoves.Count; i++)
		{
			if(validMoves[i][0] == x && validMoves[i][1] == y) { return true; }
		}
		return false;
	}

	#endregion
}
