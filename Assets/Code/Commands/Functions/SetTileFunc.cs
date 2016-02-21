//
//  SetTileFunc.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/19/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using System;

public sealed class SetTileFunc : Function
{
	public SetTileFunc(FunctionLibrary library) : base(library)
	{
	}

	public override void Compute(string[] args, Entity entity)
	{
		if (args.Length != 4) return;

		int x, y;

		if (!GetInteger(args[1], out x)) return;
		if (!GetInteger(args[2], out y)) return;

		Tile tile = TileStore.GetTileByName(args[3]);

		if (tile == null)
			return;

		Vector2i pos = new Vector2i(x, y);
		boardEditor.SetTile(pos, tile);
	}
}
