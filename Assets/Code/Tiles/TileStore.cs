﻿using UnityEngine;
using System.Collections.Generic;

public sealed class TileStore 
{
	public static readonly Tile Air = new AirTile();
	public static readonly Tile Grass = new GrassTile();
	public static readonly Tile Start = new StartTile();
	public static readonly Tile Stone = new StoneTile();
	public static readonly Tile Sand = new SandTile();
	public static readonly Tile Trigger = new TriggerTile();
	public static readonly Tile LandTrigger = new LandTriggerTile();
	public static readonly Tile Card = new CardTile();

	private static readonly List<Tile> tilesList = new List<Tile>()
	{
		Air, Grass, Start, Stone, Sand, Trigger, LandTrigger, Card
	};

	private static readonly Dictionary<string, Tile> tilesByName = new Dictionary<string, Tile>()
	{
		{ "Air", Air },
		{ "Grass", Grass },
		{ "Start", Start },
		{ "Stone", Stone },
		{ "Sand", Sand },
		{ "Trigger", Trigger },
		{ "LandTrigger", LandTrigger },
		{ "Card", Card }
	};

	public static int Count { get { return tilesByName.Count; } }

	public static Tile GetTileByName(string name)
	{
		Tile tile;
		bool success = tilesByName.TryGetValue(name, out tile);
		return success ? tile : null;
	}

	public static Tile GetTileByID(int ID)
	{
		return tilesList[ID];
	}
}