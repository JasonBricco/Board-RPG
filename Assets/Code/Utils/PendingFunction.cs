using UnityEngine;
using UnityEngine.Events;

public class PendingFunction
{
	public int entityID;
	public int turnsRemaining;
	public Data data;
	public UnityAction<Data> function;

	public PendingFunction(int entityID, int turns, Data data, UnityAction<Data> function)
	{
		this.entityID = entityID;
		this.turnsRemaining = turns;
		this.data = data;
		this.function = function;
	}
}
