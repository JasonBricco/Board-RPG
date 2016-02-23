using UnityEngine;

public sealed class RandomFunction : Function
{
	public RandomFunction(FunctionLibrary library) : base(library)
	{
	}

	public override bool GetValue(string[] args, Entity entity, out Value value)
	{
		value = new Value();

		if (!CheckArgCount(args, 3, "Usage: Random[min, max]")) return false;

		int min, max;

		if (!int.TryParse(args[1], out min))
		{
			ErrorHandler.LogText("Command Error: min value must be an integer (Random).");
			return false;
		}

		if (!int.TryParse(args[2], out max))
		{
			ErrorHandler.LogText("Command Error: max value must be an integer (Random).");
			return false;
		}

		value.Int = Random.Range(min, max + 1);

		return true;
	}
}
