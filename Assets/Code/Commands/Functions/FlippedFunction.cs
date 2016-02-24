using UnityEngine;

public class FlippedFunction : Function 
{
	public FlippedFunction(FunctionLibrary library) : base(library)
	{
		type = FunctionType.Value;
	}

	public override string GetValue(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 2, "Usage: [Flipped: entityID]")) return "null";

		int entityID;

		if (!TryGetEntityID(args[1], entity, out entityID)) return "null";

		if (!TryGetEntity(entityID, entity))
			return "false";

		if (entity.IsFlipped) return "true";
		return "false";
	}
}
