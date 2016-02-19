//
//  MoveEntity.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/19/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System;

public sealed class SetTile : Function
{
	private BoardManager boardManager;

	public SetTile()
	{
		boardManager = Engine.GetComponent<BoardManager>();
	}

	public override CommandError ValidateArguments(string[] args)
	{
		if (args.Length != 4) return CommandError.InvalidArgCount;

		int x, y;

		if (!int.TryParse(args[1], out x))
			return CommandError.InvalidArgType;

		if (!int.TryParse(args[2], out y))
			return CommandError.InvalidArgType;

		if (!boardManager.InTileBounds(x, y))
			return CommandError.InvalidArgValue;

		Tile tile = TileStore.GetTileByName(args[3]);

		if (tile == null)
			return CommandError.InvalidArgValue;

		return CommandError.None;
	}

	public override void Compute(Value[] input)
	{
		boardManager.SetTile(new Vector2i(input[0].Int, input[1].Int), TileStore.GetTileByName(input[2].String));
	}
}
