using UnityEngine;
using System;

public class IfFunction : Function 
{
	public IfFunction(Map manager) : base(manager) {}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 3, "Usage: [If: bool, [function: ...]]")) return;

		bool isTrue;

		if (!GetBool(args[1], out isTrue)) return;

		if (isTrue)
		{
			Function function;

			if (boardManager.TryGetFunction(args[2], out function))
			{
				string[] newArgs = new string[args.Length - 2];
				Array.Copy(args, 2, newArgs, 0, args.Length - 2);
				function.Compute(newArgs, entity);
			}
		}
	}
}
