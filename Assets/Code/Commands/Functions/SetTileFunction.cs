using UnityEngine;
using System;

public sealed class SetTileFunction : Function
{
	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 4, "Usage: [SetData: x, y, data]")) return;

		int x, y;

		if (!GetInteger(args[1], out x)) return;
		if (!GetInteger(args[2], out y)) return;

		if (!Map.InTileBounds(x, y))
		{
			ErrorHandler.LogText("Command Error: tried to set a tile outside of the board.");
			return;
		}

		TileType tile = Map.GetTileType(args[3]);

		if (tile == null)
		{
			ErrorHandler.LogText("Command Error: could not find tile with name: " + args[5] + " (SetTile).");
			return;
		}

		Vector2i pos = new Vector2i(x, y);
		Map.SetTile(pos, new Tile(tile.ID));

		EventManager.Notify("ValidateEntities", null);
	}
}
