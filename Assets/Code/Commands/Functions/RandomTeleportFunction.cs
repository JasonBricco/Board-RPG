using UnityEngine;
using System.Collections.Generic;

public class RandomTeleportFunction : Function 
{
	public RandomTeleportFunction(Map manager) : base(manager) {}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 4, "Usage: [RandomTeleport: entityID, x, y, z, w, ...]")) return;
		
		if ((args.Length & 1) != 0) 
		{
			ErrorHandler.LogText("Command Error: RandomTeleport expects an even number of arguments.");
			return;
		}

		int entityID;

		if (!TryGetEntityID(args[1], entity, out entityID)) return;

		List<Vector2i> points = new List<Vector2i>();

		for (int i = 2; i < args.Length; i += 2)
		{
			int x, y;

			if (!GetInteger(args[i], out x)) return;
			if (!GetInteger(args[i + 1], out y)) return;

			points.Add(new Vector2i(x, y));
		}

		if (points.Count == 0) 
		{
			ErrorHandler.LogText("Command Error: No coordinates found (RandomTeleport).");
			return;
		}

		Vector2i randomPoint = points[Random.Range(0, points.Count)];

		if (!boardManager.IsPassable(randomPoint.x, randomPoint.y)) 
		{
			ErrorHandler.LogText("Command Error: Tried to teleport to an impassable tile (RandomTeleport).");
			return;
		}

		randomPoint.x *= TileType.Size;
		randomPoint.y *= TileType.Size;

		if (TryGetEntity(entityID, entity))
			entity.SetTo(new Vector3(randomPoint.x, randomPoint.y));
	}
}
