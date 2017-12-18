/*
 *	Written by James Leahy. (c) 2017 DeFunc Art.
 *	https://github.com/defuncart/
 */
using DeFuncArt.ExtensionMethods;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>The game board.</summary>
public class GameBoard : MonoBehaviour
{
	#region Properies

	/// <summary>The number of rows.</summary>
	public const int ROWS = 8;
	/// <summary>The number of columns.</summary>
	public const int COLUMNS = 8;
	/// <summary>The BoardCell prefab.</summary>
	public BoardCell boardCellPrefab;
	/// <summary>An array of board cells.</summary>
	private BoardCell[,] cells;
	/// <summary>The BoardPiece prefab.</summary>
	public BoardPiece boardPiecePrefab;
	/// <summary>An array of board pieces.</summary>
	public BoardPiece[,] pieces { get; private set; }

	#endregion

	#region Initialization

	/// <summary>EDITOR ONLY: Callback when the script is loaded or a value is changed in the inspector.</summary>
	private void OnValidate()
	{
		Assert.IsNotNull(boardCellPrefab);
		Assert.IsNotNull(boardPiecePrefab);
	}

	/// <summary>Callback when the instance awakes.</summary>
	private void Awake()
	{
		cells = new BoardCell[COLUMNS, ROWS];
		pieces = new BoardPiece[COLUMNS, ROWS];
	}

	/// <summary>Callback when the instance starts.</summary>
	private void Start()
	{
		CreateBoardCells();
		CreateBoardPieces();
	}

	#endregion

	#region Board Initialization

	/// <summary>Resets the gameboard. </summary>
	public void Reset()
	{
		//delete any pieces on the board
		for(int x=0; x < COLUMNS; x++)
		{
			for(int y=0; y < ROWS; y++)
			{
				if(pieces[x, y] != null)
				{
					Destroy(pieces[x, y].gameObject);
				}
			}
		}
		//re-create the board pieces
		CreateBoardPieces();
		//and reset any cell highlights
		ResetBoardCellsHighlight();
	}

	/// <summary>Creates the board cells.</summary>
	private void CreateBoardCells() 
	{		
		for(int x=0; x < COLUMNS; x++) 
		{
			for(int y=0; y < ROWS; y++)
			{
				BoardCell cell = Instantiate(boardCellPrefab, this.transform, false) as BoardCell;
				cell.SetXY(x, y);
				cell.color = (x % 2 == y % 2 ? GameColors.cellDark : GameColors.cellLight);
				cells[x, y] = cell;
			}
		}
	}

	/// <summary>Creates the board pieces.</summary>
	private void CreateBoardPieces() 
	{	
		//the initial 2-row piece layout
		BoardPiece.Type[] initialPieceTypes = { BoardPiece.Type.Rook, BoardPiece.Type.Knight, BoardPiece.Type.Bishop, BoardPiece.Type.Queen, BoardPiece.Type.King, BoardPiece.Type.Bishop, BoardPiece.Type.Knight, BoardPiece.Type.Rook,
			BoardPiece.Type.Pawn, BoardPiece.Type.Pawn, BoardPiece.Type.Pawn, BoardPiece.Type.Pawn, BoardPiece.Type.Pawn, BoardPiece.Type.Pawn, BoardPiece.Type.Pawn, BoardPiece.Type.Pawn};

		int pieceCount = 0;
		for(int y=0; y < 2; y++)
		{
			for(int x=0; x < COLUMNS; x++)
			{
				BoardPiece piece = Instantiate(boardPiecePrefab, this.transform, false) as BoardPiece;
				piece.SetUp(initialPieceTypes[pieceCount], Player.Color.White, x, y);
				pieces[x, y] = piece;

				pieceCount++;
			}
		}

		pieceCount = 0;
		for(int y = ROWS-1; y >= ROWS-2; y--)
		{
			for(int x=COLUMNS-1; x >= 0; x--)
			{
				BoardPiece piece = Instantiate(boardPiecePrefab, this.transform, false) as BoardPiece;
				piece.gameObject.transform.eulerAngles = new Vector3(0, 0, 180);
				piece.SetUp(initialPieceTypes[pieceCount], Player.Color.Black, x, y);
				pieces[x, y] = piece;

				pieceCount++;
			}
		}
	}

	#endregion

	#region Move

	/// <summary>Tries to move the player's selected piece to a selected board cell.</summary>
	/// <returns><c>true</c>, if player move was possible, <c>false</c> otherwise.</returns>
	/// <param name="player">The player.</param>
	/// <param name="cellX">The x-value of the cell to move to.</param>
	/// <param name="cellY">The y-value of the cell to move to.</param>
	public bool TryPlayerMove(Player player, int cellX, int cellY)
	{
		player.selectedCell = cells[cellX, cellY];
		return TryPlayerMove(player);
	}

	/// <summary>Tries to move the player's selected piece to a selected board cell.</summary>
	/// <returns><c>true</c>, if player move was possible, <c>false</c> otherwise.</returns>
	/// <param name="player">The player.</param>
	public bool TryPlayerMove(Player player)
	{
		int targetX = player.selectedCell.x; int targetY = player.selectedCell.y;
		int pieceX = player.selectedPiece.x; int pieceY = player.selectedPiece.y;

		if(pieces[targetX, targetY] == null) //no piece at the selected cell
		{
			if(player.IsValidMove(targetX, targetY))
			{
				MovePieceFromTo(pieceX, pieceY, targetX, targetY);
				ResetBoardCellsHighlight();
				return true;
			}
			else { Debug.Log("Invalid Move"); }
		}
		else //piece at selected cell
		{
			if(player.IsValidMove(targetX, targetY))
			{
				if(pieces[targetX, targetY].canBeCaptured)
				{
					PlayerRemovesPiece(player, pieces[targetX, targetY]);
					MovePieceFromTo(pieceX, pieceY, targetX, targetY);
					ResetBoardCellsHighlight();
					return true;
				}
				else { Debug.LogFormat("{0} at ({1}, {2}) cannot be captured.", pieces[targetX, targetY].name, targetX, targetY); }
			}
			else { Debug.Log("Invalid Move"); }

		}
		return false;
	}

	/// <summary>Removes a given piece from the game board by a given player.</summary>
	/// <param name="player">The player.</param>
	/// <param name="piece">The piece.</param>
	private void PlayerRemovesPiece(Player player, BoardPiece piece)
	{
		player.AddCapturedPiece(piece);
		piece.RemovedFromBoard();
		pieces[piece.x, piece.y] = null;
	}
		
	/// <summary>Moves a board piece from (srcX, srcY) to (targetX, targetY).</summary>
	/// <param name="srcX">Source x.</param>
	/// <param name="srcY">Source y.</param>
	/// <param name="targetX">Target x.</param>
	/// <param name="targetY">Target y.</param>
	private void MovePieceFromTo(int srcX, int srcY, int targetX, int targetY)
	{
		//move the piece
		pieces[targetX, targetY] = pieces[srcX, srcY];
		pieces[srcX, srcY] = null;
		pieces[targetX, targetY].SetXY(targetX, targetY); pieces[targetX, targetY].Moved();
		//check if it can be promoted
		if(pieces[targetX, targetY].canBePromoted && 
			((pieces[targetX, targetY].isWhite && targetY == ROWS-1) || (pieces[targetX, targetY].isBlack && targetY == 0)))
		{ 
			//show promotion option
			pieces[targetX, targetY].PromoteTo(BoardPiece.Type.Queen);
		}
		//reset cells highlight
		ResetBoardCellsHighlight();
	}

	#endregion

	#region Cell Highlight

	/// <summary>Highlights the board cells of possible moves for a player's selected piece.</summary>
	public void HighlightBoardCellsForPlayer(Player player)
	{
		BoardPiece boardPiece = player.selectedPiece.GetComponent<BoardPiece>();
		player.validMoves = boardPiece.GetPlayerMovesForGameBoardPieces(player, pieces);
		foreach(int[] move in player.validMoves)
		{
			cells[move[0], move[1]].color = GameColors.cellHighlight;
		}

		if(player.validMoves.Count == 0) { cells[boardPiece.x, boardPiece.y].color = GameColors.cellNoMoves; }
	}

	/// <summary>Resets the highlight of board cells.</summary>
	public void ResetBoardCellsHighlight()
	{
		for(int x=0; x < COLUMNS; x++)
		{
			for(int y=0; y < ROWS; y++)
			{
				if(cells[x, y].color == GameColors.cellHighlight || cells[x, y].color == GameColors.cellNoMoves)
				{
					cells[x, y].color = (x % 2 == y % 2 ? GameColors.cellDark : GameColors.cellLight);
				}
			}
		}
	}

	#endregion

	#region Player

	/// <summary>Gets a list of board pieces for a given player.</summary>
	/// <returns>The player's board pieces.</returns>
	/// <param name="player">The player.</param>
	public List<BoardPiece> GetBoardPiecesForPlayer(Player player)
	{
		List<BoardPiece> playerPieces = new List<BoardPiece>();
		for(int x=0; x < GameBoard.COLUMNS; x++)
		{
			for(int y=0; y < GameBoard.ROWS; y++)
			{
				if(pieces[x, y] != null && pieces[x, y].color == player.color)
				{
					playerPieces.Add(pieces[x, y]);
				}
			}
		}
		return playerPieces;
	}

	#endregion
}
