using UnityEngine;
using System;

public sealed class TeleportFunction : Function
{
	public TeleportFunction(BoardManager manager) : base(manager) {}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 4, "Usage: [Teleport: entityID, x, y]")) return;

		int entityID, x, y;

		if (!TryGetEntityID(args[1], entity, out entityID)) return;

		if (!GetInteger(args[2], out x)) return;
		if (!GetInteger(args[3], out y)) return;

		if (!boardManager.IsPassable(x, y)) 
		{
			ErrorHandler.LogText("Command Error: attempted to move the entity to an impassable tile.");
			return;
		}

		x *= TileType.Size;
		y *= TileType.Size;

		if (TryGetEntity(entityID, entity)) entity.SetTo(new Vector3(x, y));
	}
}
