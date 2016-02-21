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
		if (args.Length < 3) return;

		int entityID;

		if (args[1] == "@")
			entityID = entity.EntityID;
		else
		{
			if (!GetInteger(args[1], out entityID))
				return;
		}

		List<Vector2i> points = new List<Vector2i>();

		for (int i = 2; i < args.Length; i++)
		{
			string[] point = args[i].Split(bracketSeparators, System.StringSplitOptions.RemoveEmptyEntries);

			if (point.Length != 2) continue;

			int x, y;

			if (!int.TryParse(point[0], out x)) continue;
			if (!int.TryParse(point[1], out y)) continue;

			points.Add(new Vector2i(x, y));
		}

		if (points.Count == 0) return;

		Vector2i randomPoint = points[Random.Range(0, points.Count)];

		Tile tile = boardManager.GetTileSafe(0, randomPoint.x, randomPoint.y);

		if (tile.ID == 0) return;

		randomPoint.x *= Tile.Size;
		randomPoint.y *= Tile.Size;

		Entity entityFromID = playerManager.GetEntity(entityID);

		if (entityFromID != null)
			entityFromID.SetTo(new Vector3(randomPoint.x, randomPoint.y));
	}
}
