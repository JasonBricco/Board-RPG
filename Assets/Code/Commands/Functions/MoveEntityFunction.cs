using UnityEngine;
using System;

public sealed class MoveEntityFunction : Function
{
	public MoveEntityFunction(FunctionLibrary library) : base(library)
	{
	}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 4, "Usage: MoveEntity(entityID, x, y)")) return;

		int entityID, x, y;

		if (!TryGetEntityID(args[1], entity, out entityID)) return;

		if (!GetInteger(args[2], entity, out x))
		{
			ErrorHandler.LogText("Command Error: x coordinate must be an integer (MoveEntity).");
			return;
		}
			
		if (!GetInteger(args[3], entity, out y))
		{
			ErrorHandler.LogText("Command Error: y coordinate must be an integer (MoveEntity).");
			return;
		}

		Tile tile = boardManager.GetTileSafe(0, x, y);

		if (tile.ID == 0) 
		{
			ErrorHandler.LogText("Command Error: attempted to move the entity to an invalid tile.");
			return;
		}

		x *= Tile.Size;
		y *= Tile.Size;

		if (TryGetEntity(entityID, entity)) entity.SetTo(new Vector3(x, y));
	}
}
