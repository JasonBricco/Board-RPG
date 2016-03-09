using UnityEngine;

public sealed class Enemy : Entity 
{
	public override void BeginTurn()
	{
		if (skipTurn) 
		{
			skipTurn = false;
			manager.NextTurn();
		}
		else
		{
			if (!beingDeleted)
				manager.NextTurn();
		}
	}
}
