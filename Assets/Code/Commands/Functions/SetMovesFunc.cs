//
//  SetMovesFunc.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/21/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;

public sealed class SetMovesFunc : Function
{
	public SetMovesFunc(FunctionLibrary library) : base(library)
	{
	}

	public override void Compute(string[] args, Entity entity)
	{
		if (args.Length != 3) return;

		int entityID, moves;

		if (args[1] == "@")
			entityID = entity.EntityID;
		else
		{
			if (!GetInteger(args[1], out entityID))
				return;
		}

		if (!GetInteger(args[2], out moves))
			return;

		moves = Mathf.Clamp(moves, 0, 100);

		Entity entityFromID = playerManager.GetEntity(entityID);

		if (entityFromID != null)
			entityFromID.RemainingMoves = moves;
	}
}
