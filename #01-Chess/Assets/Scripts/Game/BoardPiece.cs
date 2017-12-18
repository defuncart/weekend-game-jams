/*
 *	Written by James Leahy. (c) 2017 DeFunc Art.
 *	https://github.com/defuncart/
 */
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

/// <summary>A board piece can be placed on a board cell of a game board.</summary>
public class BoardPiece : BoardComponent
{
	#region Properties

	/// <summary>An enum denoting the types of board pieces.</summary>
	public enum Type
	{
		/// <summary>The king board piece type.</summary>
		King, 
		/// <summary>The queen board piece type.</summary>
		Queen, 
		/// <summary>The rook board piece type.</summary>
		Rook, 
		/// <summary>The bishop board piece type.</summary>
		Bishop, 
		/// <summary>The knight board piece type.</summary>
		Knight, 
		/// <summary>The pawn board piece type.</summary>
		Pawn
	}
	/// <summary>The piece's type. </summary>
	public Type type { get; private set; }
	/// <summary>Whether the piece is a king.</summary>
	public bool isKing { get { return type == Type.King; } }
	/// <summary>Whether the piece is a queen.</summary>
	public bool isQueen { get { return type == Type.Queen; } }
	/// <summary>Whether the piece is a rook.</summary>
	public bool isRook { get { return type == Type.Rook; } }
	/// <summary>Whether the piece is a bishop.</summary>
	public bool isBishop { get { return type == Type.Bishop; } }
	/// <summary>Whether the piece is a knight.</summary>
	public bool isKnight { get { return type == Type.Knight; } }
	/// <summary>Whether the piece is a pawn.</summary>
	public bool isPawn { get { return type == Type.Pawn; } }

	/// <summary>The piece's color.</summary>
	public Player.Color color { get; private set; }
	/// <summary>Determines if the player is playing as white.</summary>
	public bool isWhite { get { return color == Player.Color.White; } }
	/// <summary>Determines if the player is playing as black.</summary>
	public bool isBlack { get { return color == Player.Color.Black; } }

	/// <summary>Whether the piece can be captured.</summary>
	public bool canBeCaptured { get { return !isKing; } }
	/// <summary>Whether the piece has already been promoted.</summary>
	private bool hasBeenPromoted = false;
	/// <summary>Whether the piece can be promoted.</summary>
	public bool canBePromoted { get { return type == Type.Pawn && !hasBeenPromoted; } }
	/// <summary>Whether the piece can jump over other pieces.</summary>
	private bool canJump { get { return isKnight; } }

	/// <summary>The number of times the piece has been moved.</summary>
	private int timesMoved = 0;
	/// <summary>Whether the pice has already been moved.</summary>
	private bool hasBeenAlreadyMoved { get { return timesMoved != 0; } }

	/// <summary>An array of piece sprites.</summary>
	[Tooltip("Organize as Black then White, King, Queen, Rook, Bishop, Knight, Pawn.")]
	[SerializeField] private Sprite[] sprites;
	/// <summary>The sprite renderer.</summary>
	private SpriteRenderer spriteRenderer;

	#endregion

	#region Initialization

	/// <summary>EDITOR ONLY: Callback when the script is loaded or a value is changed in the inspector.</summary>
	private void OnValidate()
	{
		Assert.IsNotNull(sprites);
		for(int i=0; i < sprites.Length; i++)
		{
			Assert.IsNotNull(sprites[i]);
		}
		Assert.IsTrue(sprites.Length == Constants.NUMBER_TYPE_PIECES*2);
	}

	/// <summary>Sets the piece up.</summary>
	/// <param name="type">The piece's type.</param>
	/// <param name="color">The piece's color.</param>
	/// <param name="x">The x-value.</param>
	/// <param name="y">The y-value.</param>
	/// <param name="automaticallyUpdateTransform">Whether the transform's position should be updated. Defaults to true.</param>
	public void SetUp(BoardPiece.Type type, Player.Color color, int x, int y, bool automaticallyUpdateTransform = true)
	{
		//set the type, color, (x, y) and name
		this.type = type;
		this.color = color;
		SetXY(x, y, automaticallyUpdateTransform);
		name = string.Format("{0} {1}", type.ToString(), color.ToString());
		//update the sprite
		UpdateSprite();
	}

	#endregion

	#region Methods

	/// <summary>Updates the piece's sprite.</summary>
	private void UpdateSprite()
	{
		//get a reference to the sprite renderer if necessary
		if(spriteRenderer == null) { spriteRenderer = GetComponent<SpriteRenderer>(); }
		//determine array index
		int index = (isWhite ? 0 : Constants.NUMBER_TYPE_PIECES) + (int)type;
		spriteRenderer.sprite = sprites[index];
	}

	/// <summary>An array of move directions for the piece.</summary>
	private List<int[]> moveDirections
	{
		get
		{
			List<int[]> list = new List<int[]>();
			if(isKing) //king can move up, down, left, right and diagonally by one unit
			{
				for(int i=-1; i <= 1; i++)
				{
					for(int j=-1; j <= 1; j++)
					{
						if( i != 0 && j != 0) { list.Add(new int[]{ i, j }); }
					}
				}
			}
			else if(isQueen) //queen can move horizontally left or right by any value, vertically up or down by any value or diagonally leftup, leftdown, rightup, rightdown by any value
			{
				for(int i=1; i < GameBoard.ROWS; i++)
				{
					list.Add(new int[]{ i, 0 }); list.Add(new int[]{ 0, i }); list.Add(new int[]{ -i, 0 }); list.Add(new int[]{ 0, -i });
				}
				for(int i=1; i < GameBoard.ROWS; i++)
				{
					list.Add(new int[]{ i, i }); list.Add(new int[]{ -i, -i }); list.Add(new int[]{ -i, i }); list.Add(new int[]{ i, -i });
				}
			}
			else if(isRook) //rook can move horizontally left or right by any value or vertically up or down by any value
			{
				for(int i=1; i < GameBoard.ROWS; i++)
				{
					list.Add(new int[]{ i, 0 }); list.Add(new int[]{ 0, i }); list.Add(new int[]{ -i, 0 }); list.Add(new int[]{ 0, -i });
				}
			}
			else if(isBishop) //bishop can move diagonally leftup, leftdown, rightup, rightdown by any value
			{
				for(int i=1; i < GameBoard.ROWS; i++)
				{
					list.Add(new int[]{ i, i }); list.Add(new int[]{ -i, -i }); list.Add(new int[]{ -i, i }); list.Add(new int[]{ i, -i });
				}
			}
			else if(isKnight)
			{
				list.Add(new int[]{ -1, -2 }); list.Add(new int[]{ -2, -1 }); list.Add(new int[]{ -2, 1 }); list.Add(new int[]{ -1, 2 }); list.Add(new int[]{ 1, -2 }); list.Add(new int[]{ 2, -1 }); list.Add(new int[]{ 2, 1 }); list.Add(new int[]{ 1, 2 });
			}
			else if(isPawn) //pawn can generally just move one square forwards, two on the first move
			{
				list.Add(new int[]{ 0, (isWhite ? 1 : -1) }); //one square fowards
				if(!hasBeenAlreadyMoved) { list.Add(new int[]{ 0, (isWhite? 2 : -2) }); } //two square forwards on first move
				list.Add(new int[]{ -1, (isWhite ? 1 : -1) }); //diagonal capture left
				list.Add(new int[]{ 1, (isWhite ? 1 : -1) }); //diagonal capture right
			}
			return list;
		}
	}

	/// <summary>Determines a list of valid moves for a given player with a given board setup.</summary>
	/// <returns>A list of the valid moves for the piece.</returns>
	/// <param name="player">The player.</param>
	/// <param name="pieces">Teh gameboard pieces.</param>
	public List<int[]> GetPlayerMovesForGameBoardPieces(Player player, BoardPiece[,] pieces)
	{
		List<int[]> validMoves = new List<int[]>();

		//loop through the possible movement directions for the piece
		List<int[]> movementDirections = moveDirections;
		for(int k=0; k < movementDirections.Count; k++)
		{
			Assert.IsFalse(movementDirections[k][0] == 0 && movementDirections[k][1] == 0);

			int testX = x + movementDirections[k][0]; int testY = y + movementDirections[k][1];

			//test for out of bounds
			if(testX < 0 || testX >= GameBoard.COLUMNS || testY < 0 || testY >= GameBoard.ROWS) { continue; }
			//test that it isn't a player's piece
			if(pieces[testX, testY] != null && pieces[testX, testY].color == player.color) { continue; }
			//test for pawn specific conditions
			if(isPawn)
			{
				if(testX != x && pieces[testX, testY] == null) { continue; } //no piece to capture diagonally forwards
				if(testX == x && testY != y && pieces[testX, testY] != null) { continue; } //cannot move vertically forwards as opponents piece is blocking
			}
			//test that the opponent's piece can be captured
			if(pieces[testX, testY] != null && pieces[testX, testY].color != player.color && !pieces[testX, testY].canBeCaptured) { continue; }

			if(!canJump) //if the piece cannot jump, test if there is a piece on the path between (x, y) and (testX, testY)
			{
				if(!PieceBlocksPathBetween(x, y, testX, testY, pieces)) { continue; }
			}
			validMoves.Add( new int[]{ testX, testY } );
		}
		return validMoves;
	}

	/// <summary>Determines if there are any pieces which block a piece moving from (x1, y1) to (x2, y2)</summary>
	/// <returns>Returns true if no pieces are located are the path bwtween (x1, y1) and (x2, y2).</returns>
	/// <param name="x1">The first x value.</param>
	/// <param name="y1">The first y value.</param>
	/// <param name="x2">The second x value.</param>
	/// <param name="y2">The second y value.</param>
	/// <param name="pieces">The gameboard's pieces.</param>
	private bool PieceBlocksPathBetween(int x1, int y1, int x2, int y2, BoardPiece[,] pieces)
	{
		Assert.IsFalse(x1 == x2 && y1 == y2);

		//determine the change in position for x and y. calculate the maximum search direction
		int distanceX = x2 - x1; int distanceY = y2 - y1;
		int max = Mathf.Max(Mathf.Abs(distanceX), Mathf.Abs(distanceY));

		//loop from 1 to max-1
		for(int inBetweenMove=1; inBetweenMove <= /*Mathf.Abs(max)*/max-1; inBetweenMove++)
		{
			//determine the current step (tX, tY) position
			int tX = distanceX == 0 ? x1 : x1 + inBetweenMove * (distanceX > 0 ? 1 : -1);
			int tY = distanceY == 0 ? y1 : y1 + inBetweenMove * (distanceY > 0 ? 1 : -1);
			//if there is a piece at this position - return false
			if(pieces[tX, tY] != null) {  return false; }
		}
		//no pieces block the path - return true
		return true;
	}


	/// <summary>The piece was moved. Updates variables.</summary>
	public void Moved()
	{
		timesMoved++;
	}

	/// <summary>The piece was removed from the board. Updates variables.</summary>
	public void RemovedFromBoard()
	{
		GetComponent<BoxCollider2D>().enabled = false;
	}

	/// <summary>Promotes the piece to a given type.</summary>
	/// <param name="type">The type to promote to.</param>
	public void PromoteTo(BoardPiece.Type type)
	{
		Assert.IsTrue(type != Type.King && type != Type.Pawn, string.Format("{0} is an invalid type to promote to", type.ToString()));

		this.type = type;
		name = string.Format("{0} (Promoted Pawn) {1}", type.ToString(), color.ToString());
		UpdateSprite();
	}

	/// <summary>Gets the text character representation of the piece.</summary>
	public string text
	{
		get
		{
			if(color == Player.Color.Black)
			{
				switch(type)
				{
				case Type.King:
					return "♚";
				case Type.Queen:
					return "♛";
				case Type.Rook:
					return "♜";
				case Type.Bishop:
					return "♝";
				case Type.Knight:
					return "♞";
				case Type.Pawn:
					return "♟";
				}
			}
			else
			{
				switch(type)
				{
				case Type.King:
					return "♔";
				case Type.Queen:
					return "♕";
				case Type.Rook:
					return "♖";
				case Type.Bishop:
					return "♗";
				case Type.Knight:
					return "♘";
				case Type.Pawn:
					return "♙";
				}
			}
			return "";
		}
	}

	#endregion
}
