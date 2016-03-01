using UnityEngine;
using System.Collections.Generic;

public sealed class SetSquareFunction : Function 
{
	public SetSquareFunction(BoardManager manager) : base(manager) {}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 6, "Usage: [SetSquare: startX, startY, endX, endY, tile]")) return;

		int startX, startY, endX, endY;

		if (!GetInteger(args[1], out startX)) return;
		if (!GetInteger(args[2], out startY)) return;
		if (!GetInteger(args[3], out endX)) return;
		if (!GetInteger(args[4], out endY)) return;

		TileType tile = boardManager.GetTileType(args[5]);

		if (tile == null) 
		{
			ErrorHandler.LogText("Command Error: could not find tile with name: " + args[5] + " (SetSquare).");
			return;
		}

		List<Vector2i> points = new List<Vector2i>();

		if (endX < startX)
		{
			int tmp = startX;
			startX = endX;
			endX = tmp;
		}

		if (endY < startY)
		{
			int tmp = startY;
			startY = endY;
			endY = tmp;
		}

		for (int x = startX; x <= endX; x++)
			points.Add(new Vector2i(x, startY));

		for (int x = startX; x <= endX; x++)
			points.Add(new Vector2i(x, endY));

		for (int y = startY + 1; y < endY; y++)
			points.Add(new Vector2i(startX, y));

		for (int y = startY + 1; y < endY; y++)
			points.Add(new Vector2i(endX, y));

		boardManager.SetMultipleTiles(points, new Tile(tile.ID));
	}
}
