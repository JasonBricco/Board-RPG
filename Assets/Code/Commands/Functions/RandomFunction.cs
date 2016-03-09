using UnityEngine;

public sealed class RandomFunction : Function
{
	public RandomFunction() { type = FunctionType.Value; }

	public override string GetValue(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 3, "Usage: [Random: min, max]")) return "null";

		int min, max;

		if (!int.TryParse(args[1], out min))
		{
			ErrorHandler.LogText("Command Error: min value must be an integer (Random).");
			return "null";
		}

		if (!int.TryParse(args[2], out max))
		{
			ErrorHandler.LogText("Command Error: max value must be an integer (Random).");
			return "null";
		}

		return Random.Range(min, max + 1).ToString();
	}
}
