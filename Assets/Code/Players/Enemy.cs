using UnityEngine;
using System.Collections;

public sealed class Enemy : Entity 
{
	public override void BeginTurn()
	{
		if (skipTurn) 
		{
			skipTurn = false;
			map.NextTurn();
		}
		else
		{
			if (!beingDeleted)
				map.NextTurn();
		}
	}
}
