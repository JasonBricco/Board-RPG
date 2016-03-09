using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public sealed class PendingFunction
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

public sealed class WaitForTurns
{	
	private List<PendingFunction> pendingFunctions = new List<PendingFunction>();

	public WaitForTurns()
	{
		EventManager.StartListening("TurnEnded", ProcessPendingList);
	}

	public void Start(int entityID, int turns, Data data, UnityAction<Data> function)
	{
		pendingFunctions.Add(new PendingFunction(entityID, turns, data, function));
	}

	private void ProcessPendingList(Data data)
	{
		int entityID = data.num;

		for (int i = pendingFunctions.Count - 1; i >= 0; i--)
		{
			PendingFunction pF = pendingFunctions[i];

			if (pF.entityID == entityID)
			{
				pF.turnsRemaining--;

				if (pF.turnsRemaining == 0)
				{
					pF.function(pF.data);
					pendingFunctions.RemoveAt(i);
				}
			}
		}
	}

	~WaitForTurns()
	{
		EventManager.StopListening("TurnEnded", ProcessPendingList);
	}
}
