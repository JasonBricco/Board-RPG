using UnityEngine;
using System.Collections.Generic;

public static class TileStore
{
	private static readonly List<TileType> tilesList = new List<TileType>();
		
	private static readonly Dictionary<string, TileType> tilesByName = new Dictionary<string, TileType>();

	static TileStore()
	{
		tilesList.Add(new AirTile());
		tilesList.Add(new BasicTile(1, "Grass", 0));
		tilesList.Add(new StartTile());
		tilesList.Add(new BasicTile(3, "Stone", 2));
		tilesList.Add(new BasicTile(4, "Sand", 3));
		tilesList.Add(new TriggerTile());
		tilesList.Add(new LandTriggerTile());
		tilesList.Add(new CardTile());
		tilesList.Add(new ArrowTile());

		for (int i = 0; i < tilesList.Count; i++)
			tilesByName.Add(tilesList[i].Name, tilesList[i]);
	}

	public static int Count { get { return tilesByName.Count; } }

	public static TileType GetTileByName(string name)
	{
		TileType tile;
		bool success = tilesByName.TryGetValue(name, out tile);
		return success ? tile : null;
	}

	public static TileType GetTileByID(int ID)
	{
		return tilesList[ID];
	}
}

public sealed class Tiles
{
	public static readonly Tile Air = new Tile(0);
	public static readonly Tile Grass = new Tile(1);
	public static readonly Tile Start = new Tile(2);
	public static readonly Tile Stone = new Tile(3);
	public static readonly Tile Sand = new Tile(4);
	public static readonly Tile Trigger = new Tile(5);
	public static readonly Tile LandTrigger = new Tile(6);
	public static readonly Tile Card = new Tile(7);
	public static readonly Tile Arrow = new Tile(8);
}
