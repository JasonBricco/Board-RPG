using UnityEngine;

public class FlipFunction : Function
{
	public FlipFunction(FunctionLibrary library) : base(library)
	{
	}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 2, "Usage: [Flip: entityID]")) return;

		int entityID;

		if (!TryGetEntityID(args[1], entity, out entityID)) return;
		if (TryGetEntity(entityID, entity)) entity.Flip();
	}
}
