﻿//
//  TimerFunc.cs
//  BoardRPG
//
//  Created by Jason Bricco on 2/21/16.
//  Copyright © 2016 Jason Bricco. All rights reserved.
//

using UnityEngine;
using System.Collections.Generic;

public sealed class WaitingFunction 
{
	public Function function;
	public string[] args;
	public int turnsRemaining;
	public Entity entity;

	public WaitingFunction(Function func, string[] args, int turnsRemaining, Entity entity)
	{
		this.function = func;
		this.args = args;
		this.turnsRemaining = turnsRemaining;
		this.entity = entity;
	}
}

public sealed class TimerFunc : Function
{
	private List<WaitingFunction> pending = new List<WaitingFunction>();

	public TimerFunc(FunctionLibrary library) : base(library)
	{
		EventManager.StartListening("NewTurn", NewTurnHandler);
	}

	public override void Compute(string[] args, Entity entity)
	{
		if (args.Length != 3)
		{
			ErrorHandler.LogText("Command Error: invalid argument count for Timer.", "Usage: Timer(turns, function)");
			return;
		}

		int turns;

		if (!GetInteger(args[1], out turns))
		{
			ErrorHandler.LogText("Command Error: turn count must be an integer (Timer)");
			return;
		}

		string[] paramArgs = args[2].Split(bracketSeparators, System.StringSplitOptions.RemoveEmptyEntries);

		Function function;

		if (library.TryGetFunction(paramArgs[0], out function))
		{
			WaitingFunction func = new WaitingFunction(function, paramArgs, turns + 1, entity);
			pending.Add(func);
		}
		else
			ErrorHandler.LogText("Command Error: function passed into timer doesn't exist: " + paramArgs[0] + ".");
	}

	private void NewTurnHandler(int entityID)
	{
		for (int i = pending.Count - 1; i >= 0; i--)
		{
			WaitingFunction func = pending[i];

			if (func.entity.EntityID == entityID)
			{
				func.turnsRemaining--;

				if (func.turnsRemaining == 0)
				{
					func.function.Compute(func.args, func.entity);
					pending.RemoveAt(i);
				}
			}
		}
	}
}
