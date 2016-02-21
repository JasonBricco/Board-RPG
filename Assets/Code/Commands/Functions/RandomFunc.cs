//
//  RandomCommand.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/21/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;

public sealed class RandomFunc : Function
{
	public RandomFunc(FunctionLibrary library) : base(library)
	{
	}

	public override bool GetValue(string[] args, out Value value)
	{
		value = new Value();

		if (args.Length != 3) return false;

		int min, max;

		if (!int.TryParse(args[1], out min))
			return false;

		if (!int.TryParse(args[2], out max))
			return false;

		value.Int = Random.Range(min, max + 1);

		return true;
	}
}
