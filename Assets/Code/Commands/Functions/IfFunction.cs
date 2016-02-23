using UnityEngine;

public class IfFunction : Function 
{
	public IfFunction(FunctionLibrary library) : base(library)
	{
	}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 2, "Usage: MoveEntity(bool, function[...])")) return;

		bool isTrue;

		if (!GetBool(args[1], entity, out isTrue))
		{
			ErrorHandler.LogText("Command Error: first argument must be true or false (If).");
			return;
		}

		if (isTrue)
		{
			string[] paramArgs = args[2].Split(bracketSeparators, System.StringSplitOptions.RemoveEmptyEntries);

			Function function;

			if (library.TryGetFunction(paramArgs[0], out function))
				function.Compute(paramArgs, entity);
		}
	}
}
