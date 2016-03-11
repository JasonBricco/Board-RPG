using UnityEngine;

public sealed class SetMPFunction : Function
{
	public override void Compute(string[] args, Entity entity)
	{
		if (!CheckArgCount(args, 3, "Usage: [SetMP: entityID, moves, true/false]")) return;

		int entityID, moves;

		if (!TryGetEntityID(args[1], entity, out entityID)) return;
		if (!GetInteger(args[2], out moves)) return;

		moves = Mathf.Clamp(moves, 0, 20);

		bool tempChange = true;

		if (args.Length >= 4)
		{
			if (args[3] == "true")
				tempChange = false;
		}

		Data data = new Data(entityID, moves);
		data.mode = tempChange;
		EventManager.Notify("SetEntityMoves", data);
	}
}
