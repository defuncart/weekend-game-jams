/*
 *	Written by James Leahy. (c) 2017 DeFunc Art.
 *	https://github.com/defuncart/
 */
using DeFuncArt.ExtensionMethods;
using DeFuncArt.Serialization;
using DeFuncArt.Utilities;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>The Game manager.</summary>
public class GameManager : MonoBehaviour
{
	#region Properties

	/// <summary>A reference to the gameboard.</summary>
	[SerializeField] private GameBoard gameboard;
	/// <summary>A reference to player 1.</summary>
	[SerializeField] private Player player1;
	/// <summary>A reference to player 2.</summary>
	[SerializeField] private Player player2;
	/// <summary>The current player depending on the current game state.</summary>
	private Player player
	{
		get { return (gameState == GameState.WaitingOnPlayer1 || gameState == GameState.Player1Move ? player1 : player2); }
	}
	/// <summary>A reference to the computer ai.</summary>
	[SerializeField] private ComputerAI computerAI;

	/// <summary>An enum denoting the various game states.</summary>
	private enum GameState
	{
		/// <summary>Waiting on Player 1 - This is when it is player 1's turn and they haven't selected a piece to move.</summary>
		WaitingOnPlayer1,
		/// <summary>Player 1 Move - This is when player 1 has selected a piece to move, waiting on board cell to move to.</summary>
		Player1Move,
		/// <summary>Waiting on Player 2 - This is when it is player 2's turn and they haven't selected a piece to move.</summary>
		WaitingOnPlayer2,
		/// <summary>Player 2 Move - This is when player 2 has selected a piece to move, waiting on board cell to move to.</summary>
		Player2Move
	}
	/// <summary>The game's state.</summary>
	private GameState gameState = GameState.WaitingOnPlayer1;
	/// <summary>Whether player 1 is human and the game is waiting on them to select a piece.</summary>
	private bool waitingOnHumanPlayer1
	{
		get { return gameState == GameState.WaitingOnPlayer1 && player1.type == Player.Type.Human; }
	}
	/// <summary>Whether player 2 is human and the game is waiting on them to move a selected piece.</summary>
	private bool humanPlayer1Move
	{
		get { return gameState == GameState.Player1Move && player1.type == Player.Type.Human; }
	}
	/// <summary>Whether player 2 is human and the game is waiting on them to select a piece.</summary>
	private bool waitingOnHumanPlayer2
	{
		get { return gameState == GameState.WaitingOnPlayer2 && player2.type == Player.Type.Human; }
	}
	/// <summary>Whether player 2 is human and the game is waiting on them to move a selected piece.</summary>
	private bool humanPlayer2Move
	{
		get { return gameState == GameState.Player2Move && player2.type == Player.Type.Human; }
	}
	/// <summary>Whether player 1 is the computer and it is their turn.</summary>
	private bool computerPlayer1Move
	{
		get { return gameState == GameState.WaitingOnPlayer1 && player1.type == Player.Type.Computer; }
	}
	/// <summary>Whether player 2 is the computer and it is their turn.</summary>
	private bool computerPlayer2Move
	{
		get { return gameState == GameState.WaitingOnPlayer2 && player2.type == Player.Type.Computer; }
	}
	/// <summary>Whether the computer is thinking.</summary>
	private bool computerIsThinking = false;
	/// <summary>The board cell layer mask.</summary>
	public LayerMask boardCellLayerMask;
	/// <summary>The board piece layer mask.</summary>
	public LayerMask boardPieceLayerMask;

	#endregion

	#region Initialization

	/// <summary>EDITOR ONLY: Callback when the script is loaded or a value is changed in the inspector.</summary>
	private void OnValidate()
	{
		Assert.IsNotNull(gameboard);
		Assert.IsNotNull(player1);
		Assert.IsNotNull(player2);
		Assert.IsNotNull(computerAI);
	}

	/// <summary>callback when the instance is started.</summary>
	private void Start()
	{
		//setup the players
		player1.SetType(PlayerPreferences.GetInt(PlayerPreferencesKeys.player1));
		player2.SetType(PlayerPreferences.GetInt(PlayerPreferencesKeys.player2));
	}

	#endregion

	#region Callbacks

	/// <summary>callback when the instance is updating.</summary>
	private void Update()
	{
		if(waitingOnHumanPlayer1 || waitingOnHumanPlayer2) //if it is a human's turn to choose a piece
		{
			if(Input.GetMouseButtonDown(0)) //and there is a touch event
			{	
				//cast a ray and determine if the player is touching a board piece
				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, boardPieceLayerMask);
				if(hit.collider != null)
				{
					//save a reference to the piece
					player.selectedPiece = hit.collider.gameObject.GetComponent<BoardPiece>();

					//ignore when the player has selected an opponents piece
					if(player.selectedPiece.color != player.color) { return; }

					//hightlight cells for the piece and update game state
					gameboard.HighlightBoardCellsForPlayer(player);
					gameState = waitingOnHumanPlayer1 ? GameState.Player1Move : GameState.Player2Move;
				}
			}
		}
		else if(humanPlayer1Move || humanPlayer2Move) //else if it is a human's turn to make a move with a selected piece
		{
			if(Input.GetMouseButtonDown(0)) //and there is a touch event
			{
				//cast a ray and determine if the player is touching a board cell
				RaycastHit2D hit = Physics2D.Raycast(Camera.main.ScreenToWorldPoint(Input.mousePosition), Vector2.zero, Mathf.Infinity, boardCellLayerMask);
				if(hit.collider != null)
				{
					//first check if the player is touching one of their own pieces - if so, use that as the new selected piece
					BoardCell cell = hit.collider.gameObject.GetComponent<BoardCell>();
					BoardPiece piece = gameboard.pieces[cell.x, cell.y];
					if(piece != null && piece.color == player.color && piece.gameObject != player.selectedPiece)
					{
						player.selectedPiece = piece; //save a reference to the piece
						gameboard.ResetBoardCellsHighlight(); //highlight cells for selected piece
						gameboard.HighlightBoardCellsForPlayer(player); //highlight cells for selected piece
					}
					else //otherwise try to move the player's piece to the selected cell
					{
						player.selectedCell = cell;
						if(gameboard.TryPlayerMove(player)) //if the move is possible, change state
						{
							gameState = humanPlayer1Move ? GameState.WaitingOnPlayer2 : GameState.WaitingOnPlayer1;
						}
					}
				}
			}
		}
		else if(computerPlayer1Move || computerPlayer2Move) //else if it is a computer move
		{
			if(!computerIsThinking)
			{
				computerIsThinking = true; //computer starts to 'think'
				//wait one second and make a random move, then update game state
				this.Invoke(()=>{
					computerAI.MakeMoveForPlayer(player);
					gameState = computerPlayer1Move ? GameState.WaitingOnPlayer2 : GameState.WaitingOnPlayer1;
					computerIsThinking = false;
				}, Duration.ONE_SECOND);
			}
		}
	}

	/// <summary>Callback when the ResetButton is pressed.</summary>
	public void ResetButtonPressed()
	{
		gameboard.Reset();
		player1.Reset(); player2.Reset();
		StopAllCoroutines();
		gameState = GameState.WaitingOnPlayer1; computerIsThinking = false;
	}

	#endregion
}
