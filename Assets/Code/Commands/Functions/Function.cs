//
//  Function.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/19/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System;

public class Function
{
	private static GameObject engine;

	protected static GameObject Engine
	{
		get 
		{
			if (engine == null)
				engine = GameObject.FindWithTag("Engine");

			return engine;
		}
	}

	public virtual CommandError ValidateArguments(string[] args)
	{
		return CommandError.None;
	}

	public virtual void Compute(Value[] input)
	{
	}
}
