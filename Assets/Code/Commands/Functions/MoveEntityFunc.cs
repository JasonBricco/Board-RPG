//
//  MoveEntityFunc.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/19/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System;

public sealed class MoveEntityFunc : Function
{
	public MoveEntityFunc(FunctionLibrary library) : base(library)
	{
	}

	public override void Compute(string[] args, Entity entity)
	{
		if (args.Length != 4) return;

		int entityID, x, y;

		if (args[1] == "@")
			entityID = entity.EntityID;
		else
		{
			if (!GetInteger(args[1], out entityID))
				return;
		}

		if (!GetInteger(args[2], out x))
			return;
			
		if (!GetInteger(args[3], out y))
			return;

		Tile tile = boardManager.GetTileSafe(0, x, y);

		if (tile.ID == 0) return;

		x *= Tile.Size;
		y *= Tile.Size;

		Entity entityFromID = playerManager.GetEntity(entityID);

		if (entityFromID != null)
			entityFromID.SetTo(new Vector3(x, y));
	}
}
