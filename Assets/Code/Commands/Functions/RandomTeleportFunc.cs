//
//  RandomTeleportFunc.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/21/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections.Generic;

public class RandomTeleportFunc : Function 
{
	public RandomTeleportFunc(FunctionLibrary library) : base(library)
	{
	}

	public override void Compute(string[] args, Entity entity)
	{
		if (args.Length < 3) 
		{
			ErrorHandler.LogText("Command Error: invalid argument count for RandomTeleport.", "Usage: RandomTeleport(entityID, [x, y], [z, w], ...)");
			return;
		}

		int entityID;

		if (args[1] == "@")
			entityID = entity.EntityID;
		else
		{
			if (!GetInteger(args[1], out entityID))
			{
				ErrorHandler.LogText("Command Error: entity ID must be an integer (RandomTeleport).");
				return;
			}
		}

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

		Entity entityFromID = playerManager.GetEntity(entityID);

		if (entityFromID != null)
			entityFromID.SetTo(new Vector3(randomPoint.x, randomPoint.y));
		else
			ErrorHandler.LogText("Command Error: could not find the entity with ID " + entityID + " (RandomTeleport).");
	}
}
