using UnityEngine;
using System.Collections.Generic;

public class RandomTeleportFunction : Function 
{
	public RandomTeleportFunction(FunctionLibrary library) : base(library)
	{
	}

	public override void Compute(string[] args, Entity entity)
	{
		if (args.Length < 3) 
		{
			ErrorHandler.LogText("Command Error: invalid argument count.", "Usage: RandomTeleport(entityID, [x, y], [z, w], ...)");
			return;
		}

		int entityID;

		if (!TryGetEntityID(args[1], entity, out entityID)) return;

		List<Vector2i> points = new List<Vector2i>();

		for (int i = 2; i < args.Length; i++)
		{
			string[] point = args[i].Split(bracketSeparators, System.StringSplitOptions.RemoveEmptyEntries);

			if (point.Length != 2) 
			{
				ErrorHandler.LogText("Invalid coordinates sent to RandomTeleport.");
				continue;
			}

			int x, y;

			if (!int.TryParse(point[0], out x)) 
			{
				ErrorHandler.LogText("Coordinates must be integers (RandomTeleport).");
				continue;
			}

			if (!int.TryParse(point[1], out y)) 
			{
				ErrorHandler.LogText("Coordinates must be integers (RandomTeleport).");
				continue;
			}

			points.Add(new Vector2i(x, y));
		}

		if (points.Count == 0) 
		{
			ErrorHandler.LogText("No coordinates found (RandomTeleport).");
			return;
		}

		Vector2i randomPoint = points[Random.Range(0, points.Count)];

		Tile tile = boardManager.GetTileSafe(0, randomPoint.x, randomPoint.y);

		if (tile.ID == 0) 
		{
			ErrorHandler.LogText("Tried to teleport to an invalid tile (RandomTeleport).");
			return;
		}

		randomPoint.x *= Tile.Size;
		randomPoint.y *= Tile.Size;

		if (TryGetEntity(entityID, entity))
			entity.SetTo(new Vector3(randomPoint.x, randomPoint.y));
	}
}
