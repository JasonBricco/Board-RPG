﻿using UnityEngine;
using System.Collections.Generic;

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
	public static readonly Tile Stopper = new Tile(9);
}
