using UnityEngine;
using System;

public sealed class TimerFunction : Function
{
	private WaitForTurns waiter = new WaitForTurns();

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

			waiter.Start(entity.EntityID, turns + 1, new Data(turns, newArgs, entity), RunTimedFunction);
		}
	}

	private void RunTimedFunction(Data data)
	{
		library.GetFunction(data.strings[0]).Compute(data.strings, data.entity);
	}
}
