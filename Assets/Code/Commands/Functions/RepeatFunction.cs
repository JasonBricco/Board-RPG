using UnityEngine;
using System;

public sealed class RepeatFunction : Function
{
	private WaitForTurns waiter = new WaitForTurns();

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

			waiter.Start(entity.EntityID, turns + 1, new Data(turns, newArgs, entity), RunTimedFunction);
		}
	}

	private void RunTimedFunction(Data data)
	{
		Entity entity = data.entity;
		library.GetFunction(data.strings[0]).Compute(data.strings, entity);
		waiter.Start(entity.EntityID, data.num, data, RunTimedFunction);
	}
}
