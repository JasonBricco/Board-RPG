//
//  TileType.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/1/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using System.Collections.Generic;

public sealed class TileStore 
{
	public static readonly Tile Air = new AirTile();
	public static readonly Tile Grass = new GrassTile();
	public static readonly Tile Start = new StartTile();
	public static readonly Tile Stone = new StoneTile();
	public static readonly Tile Sand = new SandTile();

	private static readonly Dictionary<string, Tile> tilesByName = new Dictionary<string, Tile>()
	{
		{ "Air", Air },
		{ "Grass", Grass },
		{ "Start", Start },
		{ "Stone", Stone },
		{ "Sand", Sand }
	};

	public static int Count { get { return tilesByName.Count; } }

	public static Tile GetTileByName(string name)
	{
		return tilesByName[name];
	}
}
