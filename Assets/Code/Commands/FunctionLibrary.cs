//
//  FunctionLibrary.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/18/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections.Generic;
using System;

public sealed class FunctionLibrary : MonoBehaviour
{
	private Dictionary<string, Function> functions = new Dictionary<string, Function>();

	private void Awake()
	{
		functions.Add("MoveEntity", new MoveEntityFunc(this));
		functions.Add("SetTile", new SetTileFunc(this));
		functions.Add("SetMoves", new SetMovesFunc(this));
		functions.Add("Random", new RandomFunc(this));
		functions.Add("RandomTeleport", new RandomTeleportFunc(this));
	}

	public bool TryGetFunction(string name, out Function function)
	{
		return functions.TryGetValue(name, out function);
	}
}
