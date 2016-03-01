using UnityEngine;

public class RespawnFunction : Function 
{
	public RespawnFunction(BoardManager manager) : base(manager) {}

	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 2, "Usage: [Respawn: entityID]")) return;

		int entityID;

		if (!TryGetEntityID(args[1], entity, out entityID)) return;
		if (TryGetEntity(entityID, entity)) boardManager.SpawnEntity(entity);
	}
}
