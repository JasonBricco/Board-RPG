using UnityEngine;
using System;

public sealed class TeleportFunction : Function
{
	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 4, "Usage: [Teleport: entityID, x, y]")) return;

		int entityID, x, y;

		if (!TryGetEntityID(args[1], entity, out entityID)) return;

		if (!GetInteger(args[2], out x)) return;
		if (!GetInteger(args[3], out y)) return;

		if (!Map.InTileBounds(x, y))
		{
			ErrorHandler.LogText("Command Error: cannot teleport outside of the map.");
			return;
		}

		if (!Map.GetTileType(1, x, y).IsPassable(x, y)) 
		{
			ErrorHandler.LogText("Command Error: attempted to move the entity to an impassable tile.");
			return;
		}

		x *= Tile.Size;
		y *= Tile.Size;

		Data data = new Data(entityID);
		data.position = new Vector3(x, y);

		EventManager.Notify("Teleport", data);
	}
}
