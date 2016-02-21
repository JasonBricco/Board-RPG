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

public sealed class SetTile : Function
{
	private BoardEditor boardEditor;

	public SetTile()
	{
		boardEditor = Engine.GetComponent<BoardEditor>();
	}

	public override bool ValidateArguments(string[] args, Entity entity, List<Value> values)
	{
		if (args.Length != 4) return false;

		int x, y;

		if (!int.TryParse(args[1], out x))
			return false;

		if (!int.TryParse(args[2], out y))
			return false;

		Tile tile = TileStore.GetTileByName(args[3]);

		if (tile == null)
			return false;

		values.Add(new Value(x));
		values.Add(new Value(y));
		values.Add(new Value(tile));

		return true;
	}

	public override void Compute(List<Value> input)
	{
		Vector2i pos = new Vector2i(input[0].Int, input[1].Int);
		boardEditor.SetTile(pos, input[2].tile);
	}
}
