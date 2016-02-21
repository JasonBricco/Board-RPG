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

		if (args.Length != 3) 
		{
			ErrorHandler.LogText("Command Error: invalid argument count for Random.", "Usage: Random[min, max]");
			return false;
		}

		int min, max;

		if (!int.TryParse(args[1], out min))
		{
			ErrorHandler.LogText("Command Error: min value must be an integer (Random).");
			return false;
		}

		if (!int.TryParse(args[2], out max))
		{
			ErrorHandler.LogText("Command Error: max value must be an integer (Random).");
			return false;
		}

		value.Int = Random.Range(min, max + 1);

		return true;
	}
}
