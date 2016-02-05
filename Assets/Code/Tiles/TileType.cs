//
//  TileType.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using System.Collections.Generic;

public sealed class TileType 
{
	public static readonly Tile Air = new AirTile();
	public static readonly Tile Grass = new GrassTile();
	public static readonly Tile Start = new StartTile();

	private static readonly Dictionary<string, Tile> tilesByName = new Dictionary<string, Tile>()
	{
		{ "Air", Air },
		{ "Grass", Grass },
		{ "Start", Start }
	};

	public static Tile GetTileByName(string name)
	{
		return tilesByName[name];
	}
}
