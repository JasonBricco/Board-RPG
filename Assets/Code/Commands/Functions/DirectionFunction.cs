using UnityEngine;

public class DirectionFunction : Function 
{
	public DirectionFunction(BoardManager manager) : base(manager)
	{
		type = FunctionType.Value;
	}

	public override string GetValue(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 2, "Usage: [Direction: dir]")) return "null";

		switch (args[1])
		{
		case "up":
			return "0";

		case "right":
			return "1";

		case "down":
			return "2";

		case "left":
			return "3";

		default:
			ErrorHandler.LogText("Command Error: direction must be left, right, up, or down.");
			return "null";
		}
	}
}
