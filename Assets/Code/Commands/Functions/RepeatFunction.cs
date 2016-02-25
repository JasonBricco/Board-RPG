using UnityEngine;
using System.Collections.Generic;
using System;

public sealed class RepeatFunction : Function
{
	private List<WaitingFunction> pending = new List<WaitingFunction>();
	private List<int> repeatTurns = new List<int>();

	public RepeatFunction(FunctionLibrary library) : base(library)
	{
		EventManager.StartListening("NewTurn", NewTurnHandler);
		EventManager.StartListening("StateChanged", StateChanged);
	}

	private void StateChanged(int state)
	{
		if ((GameState)state == GameState.Editing)
		{
			pending.Clear();
			repeatTurns.Clear();
		}
	}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 3, "Usage: [Repeat: turns, [function: ...]]")) return;

		int turns;

		if (!GetInteger(args[1], out turns)) return;

		Function function;

		if (library.TryGetFunction(args[2], out function))
		{
			string[] newArgs = new string[args.Length - 2];
			Array.Copy(args, 2, newArgs, 0, args.Length - 2);
			WaitingFunction func = new WaitingFunction(function, newArgs, turns + 1, entity);

			pending.Add(func);
			repeatTurns.Add(turns + 1);
		}
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
					func.turnsRemaining += (repeatTurns[i] - 1);
				}
			}
		}
	}
}
