//
//  Value.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/19/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections;

public sealed class Value
{
	public int Int;
	public string String;
	public Tile tile;

	public Value(int value)
	{
		Int = value;
	}

	public Value(string value)
	{
		String = value;
	}

	public Value(Tile value)
	{
		tile = value;
	}
}
