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
		if (args.Length != 4)
		{
			ErrorHandler.LogText("Command Error: invalid argument count for SetTile.", "Usage: SetTile(x, y, tile)");
			return;
		}

		int x, y;

		if (!GetInteger(args[1], out x))
		{
			ErrorHandler.LogText("Command Error: x coordinate must be an integer (SetTile).");
			return;
		}

		if (!GetInteger(args[2], out y))
		{
			ErrorHandler.LogText("Command Error: y coordinate must be an integer (SetTile).");
			return;
		}

		Tile tile = TileStore.GetTileByName(args[3]);

		if (tile == null)
		{
			ErrorHandler.LogText("Command Error: could not find tile with name: " + args[5] + " (SetTile).");
			return;
		}

		Vector2i pos = new Vector2i(x, y);
		boardEditor.SetSingleTile(pos, tile);
	}
}
