using UnityEngine;
using System;

public sealed class SetTileFunction : Function
{
	public SetTileFunction(FunctionLibrary library) : base(library)
	{
	}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 4, "Usage: SetTile(x, y, tile)")) return;

		int x, y;

		if (!GetInteger(args[1], entity, out x))
		{
			ErrorHandler.LogText("Command Error: x coordinate must be an integer (SetTile).");
			return;
		}

		if (!GetInteger(args[2], entity, out y))
		{
			ErrorHandler.LogText("Command Error: y coordinate must be an integer (SetTile).");
			return;
		}

		Tile tile = TileStore.GetTileByName(args[3]);

		if (tile == null)
		{
			ErrorHandler.LogText("Command Error: could not find tile with name: " + args[5] + " (SetTile).");
			return;
		}

		Vector2i pos = new Vector2i(x, y);
		boardEditor.SetSingleTile(pos, tile);
	}
}
