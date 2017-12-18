/*
 *	Written by James Leahy. (c) 2017 DeFunc Art.
 *	https://github.com/defuncart/
 */
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>A Computer AI.</summary>
public class ComputerAI : MonoBehaviour
{
	#region Properties

	/// <summary>Reference to the gameboard.</summary>
	[SerializeField] private GameBoard gameboard;

	#endregion

	#region Initialization

	/// <summary>EDITOR ONLY: Callback when the script is loaded or a value is changed in the inspector.</summary>
	private void OnValidate()
	{
		Assert.IsNotNull(gameboard);
	}

	#endregion

	#region Methods

	/// <summary>Makes a move for a given player.</summary>
	/// <param name="player">The player.</param>
	public void MakeMoveForPlayer(Player player)
	{
		MakeRandomMoveForPlayer(player);
	}

	/// <summary>Makes a random move for a given player.</summary>
	/// <param name="player">The player.</param>
	private void MakeRandomMoveForPlayer(Player player)
	{
		//get a list of the player's pieces and choose a random piece
		List<BoardPiece> playerPieces = gameboard.GetBoardPiecesForPlayer(player);
		//choose a random piece that has at least one valid move
		BoardPiece randomPiece;
		do {
			randomPiece = playerPieces[Random.Range(0, playerPieces.Count)];
			player.validMoves = randomPiece.GetPlayerMovesForGameBoardPieces(player, gameboard.pieces);
		} while(player.validMoves.Count == 0);
		//choose a random move and determine it's [dX, dY]
		int[] randomMove = player.validMoves[Random.Range(0, player.validMoves.Count)];

		Debug.Log (randomPiece.name + " (" + randomPiece.x + ", " + randomPiece.y + ") -> (" + randomMove [0] + " , " + randomMove [1] + ")");

		player.selectedPiece = randomPiece;
		gameboard.TryPlayerMove(player, randomMove[0], randomMove[1]);
	}

	#endregion
}
