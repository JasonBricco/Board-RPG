using UnityEngine;

public class FlipFunction : Function
{
	public FlipFunction(FunctionLibrary library) : base(library)
	{
	}

	public override void Compute(string[] args, Entity entity)
	{
		if (args.Length < 3 || args.Length > 4) 
		{
			ErrorHandler.LogText("Command Error: invalid argument count.", "Usage: Flip(entityID, moves, *force)");
			return;
		}

		int entityID, moves;

		if (!TryGetEntityID(args[1], entity, out entityID)) return;

		if (!GetInteger(args[2], out moves))
		{
			ErrorHandler.LogText("Command Error: move count must be an integer (Flip).");
			return;
		}

		moves = Mathf.Max(moves, -1);

		bool force = false;

		if (args.Length == 4)
			force = args[3] == "force";

		if (TryGetEntity(entityID, entity)) entity.Flip(moves, force);
	}
}
