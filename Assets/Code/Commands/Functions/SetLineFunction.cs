using UnityEngine;
using System.Collections.Generic;

public sealed class SetLineFunction : Function 
{
	public SetLineFunction(FunctionLibrary library) : base(library)
	{
	}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 6, "Usage: SetLine(startX, startY, endX, endY, tile)")) return;

		int startX, startY, endX, endY;

		if (!GetInteger(args[1], out startX)) 
		{
			ErrorHandler.LogText("Command Error: startX must be an integer (SetLine).");
			return;
		}

		if (!GetInteger(args[2], out startY))
		{
			ErrorHandler.LogText("Command Error: endX must be an integer (SetLine).");
			return;
		}

		if (!GetInteger(args[3], out endX))
		{
			ErrorHandler.LogText("Command Error: startY must be an integer (SetLine).");
			return;
		}

		if (!GetInteger(args[4], out endY))
		{
			ErrorHandler.LogText("Command Error: endY must be an integer (SetLine).");
			return;
		}

		Tile tile = TileStore.GetTileByName(args[5]);

		if (tile == null)
		{
			ErrorHandler.LogText("Command Error: could not find tile with name: " + args[5] + " (SetLine).");
			return;
		}

		List<Vector2i> points = new List<Vector2i>();

		Vector2 start = new Vector2(startX, startY);
		Vector2 end = new Vector2(endX, endY);

		float dist = Vector2.Distance(start, end);

		for (int i = 0; i <= (int)dist; i++)
		{
			Vector2 point = Vector2.MoveTowards(start, end, i);
			points.Add(new Vector2i(point));
		}
			
		boardEditor.SetMultipleTiles(points, tile);
	}
}
