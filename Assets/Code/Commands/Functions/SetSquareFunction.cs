using UnityEngine;
using System.Collections.Generic;

public sealed class SetSquareFunction : Function 
{
	public SetSquareFunction(FunctionLibrary library) : base(library)
	{
	}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 6, "Usage: SetSquare(startX, startY, endX, endY, tile)")) return;

		int startX, startY, endX, endY;

		if (!GetInteger(args[1], entity, out startX)) 
		{
			ErrorHandler.LogText("Command Error: startX must be an integer (SetSquare).");
			return;
		}

		if (!GetInteger(args[2], entity, out startY))
		{
			ErrorHandler.LogText("Command Error: endX must be an integer (SetSquare).");
			return;
		}

		if (!GetInteger(args[3], entity, out endX))
		{
			ErrorHandler.LogText("Command Error: startY must be an integer (SetSquare).");
			return;
		}

		if (!GetInteger(args[4], entity, out endY))
		{
			ErrorHandler.LogText("Command Error: endY must be an integer (SetSquare).");
			return;
		}

		Tile tile = TileStore.GetTileByName(args[5]);

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

		boardEditor.SetMultipleTiles(points, tile);
	}
}
