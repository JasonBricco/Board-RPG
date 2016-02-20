//
//  MoveEntity.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/19/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System;
using System.Collections.Generic;

public sealed class MoveEntity : Function
{
	private BoardManager boardManager;
	private PlayerManager playerManager;

	public MoveEntity()
	{
		boardManager = Engine.GetComponent<BoardManager>();
		playerManager = Engine.GetComponent<PlayerManager>();
	}

	public override bool ValidateArguments(string[] args, List<Value> values)
	{
		if (args.Length != 4) return false;

		int entityID, x, y;

		if (!int.TryParse(args[1], out entityID))
			return false;

		if (!int.TryParse(args[2], out x))
			return false;
			
		if (!int.TryParse(args[3], out y))
			return false;

		Tile tile = boardManager.GetTileSafe(x, y);

		if (tile.ID == 0)
			return false;

		values.Add(new Value(entityID));
		values.Add(new Value(x));
		values.Add(new Value(y));
		
		return true;
	}

	public override void Compute(List<Value> input)
	{
		int wX = input[1].Int * Tile.Size;
		int wY = input[2].Int * Tile.Size;

		playerManager.GetEntity(input[0].Int).SetTo(new Vector3(wX, wY));
	}
}
