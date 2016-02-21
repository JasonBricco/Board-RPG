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
	protected FunctionLibrary library;
	private static GameObject engine;

	protected Char[] bracketSeparators = new char[] { '[', '/', ']' };

	protected PlayerManager playerManager
	{
		get { return Engine.Instance.GetComponent<PlayerManager>(); }
	}

	protected BoardManager boardManager
	{
		get { return Engine.Instance.GetComponent<BoardManager>(); }
	}

	protected BoardEditor boardEditor
	{
		get { return Engine.Instance.GetComponent<BoardEditor>(); }
	}

	public Function(FunctionLibrary library)
	{
		this.library = library;
	}

	public virtual void Compute(string[] args, Entity entity)
	{
	}

	public virtual bool GetValue(string[] args, out Value value)
	{
		value = new Value();
		return true;
	}

	protected bool GetInteger(string arg, out int num)
	{
		if (int.TryParse(arg, out num))
			return true;

		string[] args = arg.Split(bracketSeparators, StringSplitOptions.RemoveEmptyEntries);

		Function function;
		Value value;

		if (library.TryGetFunction(args[0], out function))
		{
			if (function.GetValue(args, out value))
			{
				num = value.Int;
				return true;
			}
		}

		num = 0;
		return false;
	}
}
