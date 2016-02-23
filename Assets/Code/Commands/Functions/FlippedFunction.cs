using UnityEngine;

public class FlippedFunction : Function 
{
	public FlippedFunction(FunctionLibrary library) : base(library)
	{
	}

	public override bool GetValue(string[] args, Entity entity, out Value value)
	{
		value = new Value();

		if (!CheckArgCount(args, 2, "Usage: Flipped[entityID]")) return false;

		int entityID;

		if (!TryGetEntityID(args[1], entity, out entityID)) return false;

		if (!TryGetEntity(entityID, entity))
			return false;

		value.Bool = entity.IsFlipped;
		return true;
	}
}
