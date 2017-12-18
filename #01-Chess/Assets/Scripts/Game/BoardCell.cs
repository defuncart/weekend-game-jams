/*
 *	Written by James Leahy. (c) 2017 DeFunc Art.
 *	https://github.com/defuncart/
 */
using UnityEngine;

/// <summary>A board cell of a game board.</summary>
public class BoardCell : BoardComponent
{
	/// <summary>The sprite renderer.</summary>
	private SpriteRenderer spriteRenderer;

	/// <summary>Callback when the instance awakes.</summary>
	private void Awake()
	{
		spriteRenderer = GetComponent<SpriteRenderer>();
	}

	/// <summary>Sets the x and y values.</summary>
	/// <param name="x">The x value.</param>
	/// <param name="y">The y value.</param>
	/// <param name="automaticallyUpdateTransform">Whether the transform's position should be updated. Defaults to false.</param>
	public override void SetXY(int x, int y, bool automaticallyUpdateTransform = true)
	{
		base.SetXY(x, y, automaticallyUpdateTransform);
		name = string.Format("BoardCell ({0}, {1})", x.ToString(), y.ToString());
	}

	/// <summary>The cell's color.</summary>
	public Color color
	{
		get { return spriteRenderer.color; }
		set { spriteRenderer .color = value; }
	}
}
