using UnityEngine;
using System;

public sealed class RepeatFunction : Function
{
	public RepeatFunction(BoardManager manager) : base(manager) {}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 3, "Usage: [Repeat: turns, [function: ...]]")) return;

		int turns;

		if (!GetInteger(args[1], out turns)) return;

		Function function;

		if (boardManager.TryGetFunction(args[2], out function))
		{
			string[] newArgs = new string[args.Length - 2];
			Array.Copy(args, 2, newArgs, 0, args.Length - 2);

			Data data = new Data();
			data.strings = newArgs;
			data.num0 = entity.EntityID;
			data.num1 = turns;

			boardManager.WaitForTurns(entity.EntityID, turns + 1, data, RunTimedFunction);
		}
	}

	private void RunTimedFunction(Data data)
	{
		Entity entity = boardManager.GetEntity(data.num0);
		boardManager.GetFunction(data.strings[0]).Compute(data.strings, entity);
		boardManager.WaitForTurns(entity.EntityID, data.num1, data, RunTimedFunction);
	}
}
