using UnityEngine;
using System.Collections.Generic;

public class RandomTeleportFunction : Function 
{
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

		Utils.ShuffleList<Vector2i>(points);

		Vector2i target = new Vector2i(-1, 0);

		for (int i = 0; i < points.Count; i++)
		{
			Vector2i next = points[i];

			if (!Map.InTileBounds(next.x, next.y))
				continue;

			if (!Map.GetTileType(1, next.x, next.y).IsPassable(next.x, next.y))
				continue;

			target = next;
			break;
		}

		if (target.x == -1)
		{
			ErrorHandler.LogText("Command Error: No passable tiles supplied (RandomTeleport).");
			return;
		}

		target.x *= TileType.Size;
		target.y *= TileType.Size;

		Data data = new Data(entityID);
		data.position = new Vector3(target.x, target.y);

		EventManager.Notify("Teleport", data);
	}
}
