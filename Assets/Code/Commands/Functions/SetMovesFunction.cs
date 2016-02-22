using UnityEngine;

public sealed class SetMovesFunction : Function
{
	public SetMovesFunction(FunctionLibrary library) : base(library)
	{
	}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 3, "Usage: SetMoves(entityID, moves)")) return;

		int entityID, moves;

		if (!TryGetEntityID(args[1], entity, out entityID)) return;

		if (!GetInteger(args[2], out moves))
		{
			ErrorHandler.LogText("Command Error: move count must be an integer (SetMoves).");
			return;
		}

		moves = Mathf.Clamp(moves, 0, 100);

		if (TryGetEntity(entityID, entity)) entity.RemainingMoves = moves;
	}
}
