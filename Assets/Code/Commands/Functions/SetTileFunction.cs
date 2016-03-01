using UnityEngine;
using System;

public sealed class SetTileFunction : Function
{
	public SetTileFunction(FunctionLibrary library) : base(library) {}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 4, "Usage: [SetData: x, y, data]")) return;

		int x, y;

		if (!GetInteger(args[1], out x)) return;
		if (!GetInteger(args[2], out y)) return;

		if (!Engine.BoardManager.InTileBounds(x, y))
		{
			ErrorHandler.LogText("Command Error: tried to set a tile outside of the board.");
			return;
		}

		TileType tile = TileStore.GetTileByName(args[3]);

		if (tile == null)
		{
			ErrorHandler.LogText("Command Error: could not find tile with name: " + args[5] + " (SetTile).");
			return;
		}

		Vector2i pos = new Vector2i(x, y);
		Engine.BoardEditor.SetSingleTile(pos, new Tile(tile.ID));
	}
}
