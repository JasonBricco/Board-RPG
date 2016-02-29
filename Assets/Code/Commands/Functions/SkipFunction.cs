using UnityEngine;

public class SkipFunction : Function 
{
	public SkipFunction(FunctionLibrary library) : base(library) {}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 2, "Usage: [Skip: entityID]")) return;

		int entityID;

		if (!TryGetEntityID(args[1], entity, out entityID)) return;
		if (TryGetEntity(entityID, entity)) entity.SkipNextTurn();
	}
}
