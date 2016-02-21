//
//  StartTile.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/5/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;

public sealed class StartTile : Tile 
{
	public StartTile()
	{
		name = "Start";
		tileID = 2;
		meshIndex = 1;
		posIndex = 1;
	}

	public override bool CanAdd(BoardData data, Vector2i pos)
	{
		return data.startTiles.Count < 20;
	}

	public override void OnAdded(BoardData data, Vector2i pos)
	{
		if (!data.startTiles.Contains(pos))
			data.startTiles.Add(pos);
	}

	public override void OnDeleted(BoardData data, Vector2i pos)
	{
		data.startTiles.Remove(pos);
	}
}
