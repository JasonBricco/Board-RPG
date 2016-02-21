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
		if (args.Length != 3)
		{
			ErrorHandler.LogText("Command Error: invalid argument count for SetMoves.", "Usage: SetMoves(entityID, moves)");
			return;
		}

		int entityID, moves;

		if (args[1] == "@")
			entityID = entity.EntityID;
		else
		{
			if (!GetInteger(args[1], out entityID))
			{
				ErrorHandler.LogText("Command Error: entity ID must be an integer (SetMoves).");
				return;
			}
		}

		if (!GetInteger(args[2], out moves))
		{
			ErrorHandler.LogText("Command Error: move count must be an integer (SetMoves).");
			return;
		}

		moves = Mathf.Clamp(moves, 0, 100);

		Entity entityFromID = playerManager.GetEntity(entityID);

		if (entityFromID != null)
			entityFromID.RemainingMoves = moves;
		else
			ErrorHandler.LogText("Command Error: could not find the entity with ID " + entityID + " (SetMoves).");
	}
}
