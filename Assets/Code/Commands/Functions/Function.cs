//
//  Function.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/19/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System;
using System.Collections.Generic;

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

	public virtual bool ValidateArguments(string[] args, Entity entity, List<Value> values)
	{
		return true;
	}

	public virtual void Compute(List<Value> input)
	{
	}
}
