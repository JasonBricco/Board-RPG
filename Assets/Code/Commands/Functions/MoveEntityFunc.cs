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
		if (args.Length != 4) 
		{
			ErrorHandler.LogText("Command Error: invalid argument count for MoveEntity", "Usage: MoveEntity(entityID, x, y)");
			return;
		}

		int entityID, x, y;

		if (args[1] == "@")
			entityID = entity.EntityID;
		else
		{
			if (!GetInteger(args[1], out entityID))
			{
				ErrorHandler.LogText("Command Error: entity ID must be an integer (MoveEntity).");
				return;
			}
		}

		if (!GetInteger(args[2], out x))
		{
			ErrorHandler.LogText("Command Error: x coordinate must be an integer (MoveEntity).");
			return;
		}
			
		if (!GetInteger(args[3], out y))
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

		Entity entityFromID = playerManager.GetEntity(entityID);

		if (entityFromID != null)
			entityFromID.SetTo(new Vector3(x, y));
		else
			ErrorHandler.LogText("Command Error: could not find the entity with ID " + entityID + " (MoveEntity).");
	}
}
