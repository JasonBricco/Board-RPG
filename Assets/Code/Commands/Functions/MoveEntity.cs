//
//  MoveEntity.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/19/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System;

public sealed class MoveEntity : Function
{
	private BoardManager boardManager;
	private PlayerManager playerManager;

	public MoveEntity()
	{
		boardManager = Engine.GetComponent<BoardManager>();
		playerManager = Engine.GetComponent<PlayerManager>();
	}

	public override CommandError ValidateArguments(string[] args)
	{
		if (args.Length != 4) return CommandError.InvalidArgCount;

		int entityID, x, y;

		if (!int.TryParse(args[1], out entityID))
			return CommandError.InvalidArgType;

		if (!int.TryParse(args[2], out x))
			return CommandError.InvalidArgType;
			
		if (!int.TryParse(args[3], out y))
			return CommandError.InvalidArgType;

		if (!boardManager.InTileBounds(x, y))
			return CommandError.InvalidArgValue;

		return CommandError.None;
	}

	public override void Compute(Value[] input)
	{
		playerManager.GetEntity(input[0].Int).SetTo(new Vector3(input[1].Int, input[2].Int));
	}
}
