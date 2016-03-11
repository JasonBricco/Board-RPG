using UnityEngine;
using UnityEngine.Events;
using System.Collections.Generic;

public sealed class PendingFunction
{
	public int turnsRemaining;
	public Data data;
	public EventDelegate function;

	public PendingFunction(int turns, Data data, EventDelegate function)
	{
		this.turnsRemaining = turns;
		this.data = data;
		this.function = function;
	}
}
