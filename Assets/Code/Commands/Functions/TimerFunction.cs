using UnityEngine;
using System.Collections.Generic;
using System;

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

public sealed class TimerFunction : Function
{
	private List<WaitingFunction> pending = new List<WaitingFunction>();

	public TimerFunction(FunctionLibrary library) : base(library)
	{
		EventManager.StartListening("NewTurn", NewTurnHandler);
		EventManager.StartListening("StateChanged", StateChanged);
	}

	private void StateChanged(int state)
	{
		if ((GameState)state == GameState.Editing)
			pending.Clear();
	}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 3, "Usage: [Timer: turns, [function: ...]]")) return;

		int turns;

		if (!GetInteger(args[1], out turns)) return;

		Function function;

		if (library.TryGetFunction(args[2], out function))
		{
			string[] newArgs = new string[args.Length - 2];
			Array.Copy(args, 2, newArgs, 0, args.Length - 2);
			WaitingFunction func = new WaitingFunction(function, newArgs, turns + 1, entity);
			pending.Add(func);
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
					pending.RemoveAt(i);
				}
			}
		}
	}
}
